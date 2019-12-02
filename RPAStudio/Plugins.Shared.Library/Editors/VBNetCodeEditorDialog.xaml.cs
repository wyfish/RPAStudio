using ICSharpCode.AvalonEdit.CodeCompletion;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.VisualBasic;
using Plugins.Shared.Library.CodeCompletion;
using System;
using System.Activities;
using System.Activities.Expressions;
using System.Activities.Presentation;
using System.Activities.Presentation.Model;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Plugins.Shared.Library.Editors
{
    public partial class VBNetCodeEditorDialog
    {
        private CompletionWindow completionWindow;
        private string m_codeHeader;
        private string m_codeFooter;
        internal ExpressionNode m_namespaceNodeRoot = new ExpressionNode();
        private List<string> variableDeclarations = new List<string>();

        static VBNetCodeEditorDialog()
        {
            Task.Run(() =>
            {
                EditorUtil.init();
            });
        }

        public VBNetCodeEditorDialog(ModelItem ownerActivity)
        {
            base.ModelItem = ownerActivity;
            base.Context = ownerActivity.GetEditingContext();
            InitializeComponent();
            textEditor.Text = base.ModelItem.Properties["Code"].ComputedValue as string;
            Dictionary<string, Argument> args = base.ModelItem.Properties["Arguments"]?.ComputedValue as Dictionary<string, Argument>;
            m_codeHeader = GetCodeHeader(args);
            m_codeFooter = GetCodeFooter();

            textEditor.TextArea.TextEntering += TextArea_TextEntering;
            textEditor.TextArea.TextEntered += TextArea_TextEntered;
            textEditor.TextArea.GotFocus += TextArea_GotFocus;

            foreach(var arg in args)
            {
                variableDeclarations.Add(arg.Key);
            }

        }

        private void TextArea_GotFocus(object sender, RoutedEventArgs e)
        {
            m_namespaceNodeRoot = AddNamespacesToAutoCompletionList(EditorUtil.autoCompletionTree, GetImportsNamespaces());
        }

        private ExpressionNode AddNamespacesToAutoCompletionList(ExpressionNode data, List<string> importedNamespaces)
        {
            foreach (var ns in importedNamespaces)
            {
                var foundNodes = ExpressionNode.SearchForNode(data, ns, true, true);
                foreach (var node in foundNodes.Nodes)
                {
                    data.Add(node);
                }
            }
            return data;
        }

        private void TextArea_TextEntering(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    // Whenever a non-letter is typed while the completion window is open,
                    // insert the currently selected element.
                    completionWindow.CompletionList.RequestInsertion(e);
                }
            }
            // Do not set e.Handled=true.
            // We still want to insert the character that was typed.
        }


        private void TextArea_TextEntered(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            var completionDataList = new List<QueryCompletionData>();

            if (e.Text == ".")
            {
                //字符串不需要代码提示
                var txt = textEditor.Text.Trim();
                if ((txt.StartsWith("\"") || txt.StartsWith("“") || txt.StartsWith("”"))
                    && (txt.EndsWith("\"") || txt.EndsWith("“") || txt.EndsWith("”")))
                {
                    return;
                }

                try
                {
                    string codeString = m_codeHeader + "\r\nDim _tmp_ =" + textEditor.Text.Substring(0, textEditor.CaretOffset) + m_codeFooter;

                    var tree = VisualBasicSyntaxTree.ParseText(codeString);

                    var root = tree.GetRoot();

                    Assembly target = Assembly.GetEntryAssembly();
                    List<Assembly> references = (from assemblyName in target.GetReferencedAssemblies()
                                                 select Assembly.Load(assemblyName)).ToList();

                    var CustomIntellisense = VisualBasicCompilation.Create("CustomIntellisense");

                    foreach (var assembly in references)
                    {
                        CustomIntellisense = CustomIntellisense.AddReferences(MetadataReference.CreateFromFile(assembly.Location));
                    }

                    CustomIntellisense = CustomIntellisense.AddSyntaxTrees(tree);

                    var model = CustomIntellisense.GetSemanticModel(tree);

                    var exprSyntaxNode = root.FindToken(codeString.LastIndexOf('.') - 1).Parent;
                    var exprFullNameString = exprSyntaxNode.Parent.ToString().Trim();
                    if (exprFullNameString.EndsWith("."))
                    {
                        exprFullNameString = exprFullNameString.Substring(0, exprFullNameString.Length - 1);
                    }

                    List<ExpressionNode> rootNodes =
                        ExpressionNode.SubsetAutoCompletionList(m_namespaceNodeRoot, exprFullNameString);

                    foreach (var item in rootNodes)
                    {
                        foreach (var child_item in item.Nodes)
                        {
                            if (child_item.ItemType == "namespace")
                            {
                                completionDataList.Add(new QueryCompletionData(child_item.Name, child_item.Description, SymbolKind.Namespace));
                            }
                            else if (child_item.ItemType == "class")
                            {
                                completionDataList.Add(new QueryCompletionData(child_item.Name, child_item.Description, SymbolKind.NamedType));
                            }
                            else
                            {
                                //TODO WJF后期根据需要添加
                            }
                        }
                    }

                    var literalInfo = model.GetTypeInfo(exprSyntaxNode);
                    var stringTypeSymbol = (INamedTypeSymbol)literalInfo.Type;

                    IList<ISymbol> symbols = new List<ISymbol>() { };
                    foreach (var s in (from method in stringTypeSymbol.GetMembers() where method.DeclaredAccessibility == Microsoft.CodeAnalysis.Accessibility.Public select method).Distinct())
                        symbols.Add(s);

                    if (symbols != null && symbols.Count > 0)
                    {
                        var distinctSymbols = from s in symbols group s by s.Name into g select new { Name = g.Key, Symbols = g };
                        foreach (var g in distinctSymbols.OrderBy(s => s.Name.ToLower()))
                        {
                            completionDataList.Add(new QueryCompletionData(g.Name, g.Symbols.ToArray()));
                        }
                    }
                }
                catch { }
            }
            else
            {
                if (completionWindow == null)
                {
                    //列出本地变量列表
                    bool bShow = false;
                    if (textEditor.SelectionStart == textEditor.Text.Length)
                    {
                        if (textEditor.Text.Length == 1)
                        {
                            bShow = true;
                        }

                        if (textEditor.Text.Length >= 2)
                        {
                            var prevChar = textEditor.Text[textEditor.Text.Length - 2];
                            bShow = !char.IsLetterOrDigit(prevChar);
                        }
                    }

                    if (bShow)
                    {
                        var queryVarList = from s in variableDeclarations
                                           where s.StartsWith(e.Text, System.StringComparison.CurrentCultureIgnoreCase)
                                           select s;

                        foreach (var name in queryVarList.OrderBy(s => s.ToLower()))
                        {
                            completionDataList.Add(new QueryCompletionData(name, string.Format("本地变量 {0}", name), SymbolKind.Local));
                        }
                    }
                }



            }

            if (completionDataList.Count > 0)
            {
                completionWindow = new CompletionWindow(textEditor.TextArea);
                IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
                data.Clear();
                foreach (var item in completionDataList)
                {
                    data.Add(item);
                }

                if (e.Text != ".")
                {
                    //此处需要特殊处理下
                    completionWindow.StartOffset -= e.Text.Length;
                    completionWindow.CompletionList.SelectItem(e.Text);
                }

                completionWindow.Show();
                completionWindow.Closed += CompletionWindowClosed;
            }


        }

        private void CompletionWindowClosed(object sender, EventArgs e)
        {
            if (completionWindow != null)
            {
                completionWindow.Closed -= CompletionWindowClosed;
                completionWindow = null;
            }
        }


        protected override void OnWorkflowElementDialogClosed(bool? dialogResult)
        {
            if (dialogResult.HasValue && dialogResult.Value)
            {
                base.ModelItem.Properties["Code"].ComputedValue = textEditor.Text;
            }
        }

        private static string GetVbNetTypeName(Type t)
        {
            if (!t.IsGenericType)
            {
                return t.FullName.Replace("[]", "()");
            }
            if (t.IsNested && t.DeclaringType.IsGenericType)
            {
                throw new NotImplementedException();
            }
            string str = t.FullName.Substring(0, t.FullName.IndexOf('`')) + "(Of ";
            int num = 0;
            Type[] genericArguments = t.GetGenericArguments();
            foreach (Type t2 in genericArguments)
            {
                if (num > 0)
                {
                    str += ", ";
                }
                str += GetVbNetTypeName(t2);
                num++;
            }
            return str + ")";
        }

        public string GetVbNetArguments(List<Tuple<string, Type, ArgumentDirection>> inArgs)
        {
            string text = "";
            foreach (Tuple<string, Type, ArgumentDirection> inArg in inArgs)
            {
                if (!string.IsNullOrEmpty(text))
                {
                    text += ", ";
                }
                string arg = "";
                switch (inArg.Item3)
                {
                    case ArgumentDirection.In:
                        arg = "ByVal";
                        break;
                    case ArgumentDirection.Out:
                    case ArgumentDirection.InOut:
                        arg = "ByRef";
                        break;
                }
                text += $"{arg} {inArg.Item1} As {GetVbNetTypeName(inArg.Item2)}";
            }
            return text;
        }

        public static IEnumerable<string> EnumerateImports(EditingContext context, ModelItem modelItem)
        {
            IEnumerable<string> enumerable = context?.Services.GetService(typeof(HashSet<string>)) as HashSet<string>;
            IEnumerable<string> first = enumerable ?? Enumerable.Empty<string>();
            object obj = modelItem?.Root?.GetCurrentValue();
            object enumerable2;
            if (obj == null)
            {
                enumerable2 = Enumerable.Empty<string>();
            }
            else
            {
                enumerable = TextExpression.GetNamespacesForImplementation(obj);
                enumerable2 = enumerable;
            }
            IEnumerable<string> second = (IEnumerable<string>)enumerable2;
            return first.Union(second).Distinct();
        }

        public List<string> GetImportsNamespaces()
        {
            var ret = new List<string>();
            var imports = EnumerateImports(base.Context, base.ModelItem);

            foreach (string import in imports)
            {
                if (!string.IsNullOrWhiteSpace(import))
                {
                    ret.Add(import);
                }
            }
            return ret;
        }

        public string GetImports(IEnumerable<string> imports)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string import in imports)
            {
                if (!string.IsNullOrWhiteSpace(import))
                {
                    stringBuilder.AppendLine($"Imports {import}");
                }
            }
            return stringBuilder.ToString();
        }

        public string GetCodeHeader(Dictionary<string, Argument> args)
        {
            List<Tuple<string, Type, ArgumentDirection>> inArgs = (from a in args
                                                                   select new Tuple<string, Type, ArgumentDirection>(a.Key, a.Value.ArgumentType, a.Value.Direction)).ToList();
            string importsHeader = GetImports(EnumerateImports(base.Context, base.ModelItem));
            string vbNetArguments = GetVbNetArguments(inArgs);
            return $"{importsHeader}Module RPACodeRunner\r\nSub Run({vbNetArguments})\r\n";
        }

        public static string GetCodeFooter()
        {
            return "\r\nEnd Sub\r\nEnd Module";
        }

                
    }
}
