using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Highlighting;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using System;
using System.Activities.Presentation.Hosting;
using System.Activities.Presentation.Model;
using System.Activities.Presentation.View;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.ObjectModel;
using log4net;
using RPAStudio.Librarys;
using Plugins.Shared.Library.CodeCompletion;

namespace RPAStudio.ExpressionEditor
{
    public class RoslynExpressionEditorInstance : IExpressionEditorInstance
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private TextEditor textEditor = new TextEditor();
        private CompletionWindow completionWindow;
        private List<string> variableDeclarations = new List<string>();//变量和参数列表
        internal ExpressionNode m_namespaceNodeRoot = new ExpressionNode();
        private Collection<string> ImportedNamespaces { get; set; }

        public RoslynExpressionEditorInstance()
        {
            textEditor.TextArea.TextEntering += TextArea_TextEntering;
            textEditor.TextArea.TextEntered += TextArea_TextEntered;
            textEditor.TextArea.GotFocus += TextArea_GotFocus;
            textEditor.TextArea.LostKeyboardFocus += TextArea_LostFocus; // Need to detach events.
            textEditor.TextChanged += TextEditor_TextChanged;
            textEditor.Unloaded += TextEditor_Unloaded;

            textEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("VB");
            textEditor.FontSize = 12;

            textEditor.Padding = new Thickness(0, 0, 15, 0);//以便让最后的光标CARET显示出来，并且防止错误符号遮挡

        }

        private void TextEditor_Unloaded(object sender, RoutedEventArgs e)
        {
            Closing?.Invoke(sender, EventArgs.Empty);
        }

        private void TextArea_GotFocus(object sender, RoutedEventArgs e)
        {
            GotAggregateFocus?.Invoke(sender, e);
        }

        private void TextEditor_TextChanged(object sender, EventArgs e)
        {
            TextChanged?.Invoke(sender, e);
        }

        public RoslynExpressionEditorInstance(Size initialSize) : this()
        {
            textEditor.Width = initialSize.Width;
            textEditor.Height = initialSize.Height;
        }

        public void UpdateInstance(List<ModelItem> variables, string text, ImportedNamespaceContextItem importedNamespaces)
        {
            textEditor.Text = text;

            try
            {
                variableDeclarations = variables.Select(v =>
                {
                    var c = v.GetCurrentValue() as System.Activities.Variable;
                    //此处c可能为NULL
                    return c.Name;
                }).ToList();
            }
            catch (Exception err)
            {
                Logger.Debug(err, logger);
                variableDeclarations = new List<string>();
            }
           

            this.ImportedNamespaces = importedNamespaces.ImportedNamespaces;
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
                    var importsNamespace = "";//导入命名空间以实现省略命名空间的功能
                    foreach (var ns in ImportedNamespaces)
                    {
                        importsNamespace += string.Format("Imports {0}", ns);
                        importsNamespace += Environment.NewLine;
                    }
                     var startString = importsNamespace+@"
Module Module1

    Sub Main()
        Dim _tmp_ =";


                    var endString= @" 
    End Sub

End Module
";
                    string codeString = startString + textEditor.Text.Substring(0, textEditor.CaretOffset) + endString;

                    var tree = VisualBasicSyntaxTree.ParseText(codeString);

                    var root = tree.GetRoot();

                    //TODO WJF 动态加载组件后，此处运行是否还正常？

                    Assembly target = Assembly.GetExecutingAssembly();
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
                    if(exprFullNameString.EndsWith("."))
                    {
                        exprFullNameString = exprFullNameString.Substring(0, exprFullNameString.Length - 1);
                    }

                    List<ExpressionNode> rootNodes =
                        ExpressionNode.SubsetAutoCompletionList(m_namespaceNodeRoot, exprFullNameString);

                    foreach(var item in rootNodes)
                    {
                        foreach (var child_item in item.Nodes)
                        {
                            if(child_item.ItemType == "namespace")
                            {
                                completionDataList.Add(new QueryCompletionData(child_item.Name, child_item.Description, SymbolKind.Namespace));
                            }else if(child_item.ItemType == "class")
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
                    foreach (var s in (from method in stringTypeSymbol.GetMembers() where method.DeclaredAccessibility == Accessibility.Public select method).Distinct())
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
                if(completionWindow == null)
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

                if(e.Text != ".")
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

        private void TextArea_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.LostAggregateFocus != null)
            {
                this.LostAggregateFocus(sender, e);
            }
        }

        #region IExpressionEditorInstance implicit

        public bool AcceptsReturn { get; set; }

        public bool AcceptsTab { get; set; }

        public bool HasAggregateFocus
        {
            get
            {
                return true;
            }
        }

        public Control HostControl
        {
            get
            {
                return textEditor;
            }
        }

        public int MaxLines { get; set; }

        public int MinLines { get; set; }

        public event EventHandler Closing;
        public event EventHandler GotAggregateFocus;
        public event EventHandler LostAggregateFocus;
        public event EventHandler TextChanged;

        public bool CanCompleteWord()
        {
            return true;
        }

        public bool CanCopy()
        {
            return true;
        }

        public bool CanCut()
        {
            return true;
        }

        public bool CanDecreaseFilterLevel()
        {
            return true;
        }

        public bool CanGlobalIntellisense()
        {
            return true;
        }

        public bool CanIncreaseFilterLevel()
        {
            return true;
        }

        public bool CanParameterInfo()
        {
            return true;
        }

        public bool CanPaste()
        {
            return true;
        }

        public bool CanQuickInfo()
        {
            return true;
        }

        public void ClearSelection()
        {

        }

        public void Close()
        {

        }

        public bool CompleteWord()
        {
            return true;
        }

        public string GetCommittedText()
        {
            return textEditor.Text;
        }

        public bool GlobalIntellisense()
        {
            return true;
        }
        public bool DecreaseFilterLevel()
        {
            return true;
        }

        public bool IncreaseFilterLevel()
        {
            return true;
        }

        public bool ParameterInfo()
        {
            return true;
        }

        public bool QuickInfo()
        {
            return true;
        }

        #endregion

        #region IExpressionEditorInstance explicit

        void IExpressionEditorInstance.Focus()
        {
            textEditor.Focus();
        }

        bool IExpressionEditorInstance.Cut()
        {
            textEditor.Cut();
            return true;
        }

        bool IExpressionEditorInstance.Copy()
        {
            textEditor.Copy();
            return true;
        }

        bool IExpressionEditorInstance.Paste()
        {
            textEditor.Paste();
            return true;
        }

        bool IExpressionEditorInstance.Undo()
        {
            return textEditor.Undo();
        }

        bool IExpressionEditorInstance.Redo()
        {
            return textEditor.Redo();
        }

        bool IExpressionEditorInstance.CanUndo()
        {
            return textEditor.CanUndo;
        }

        bool IExpressionEditorInstance.CanRedo()
        {
            return textEditor.CanRedo;
        }

        string IExpressionEditorInstance.Text
        {
            get
            {
                return textEditor.Text;
            }

            set
            {
                textEditor.Text = value;
            }
        }

        ScrollBarVisibility IExpressionEditorInstance.VerticalScrollBarVisibility
        {
            get
            {
                return textEditor.VerticalScrollBarVisibility;
            }

            set
            {
                textEditor.VerticalScrollBarVisibility = value;
            }
        }

        ScrollBarVisibility IExpressionEditorInstance.HorizontalScrollBarVisibility
        {
            get
            {
                return textEditor.HorizontalScrollBarVisibility;
            }

            set
            {
                textEditor.HorizontalScrollBarVisibility = value;
            }
        }

        


        #endregion
    }
}
