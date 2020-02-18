using System;
using System.Activities;
using System.Activities.Core.Presentation;
using System.Activities.Presentation.Model;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPAStudio.Librarys
{
    /// <summary>
    /// ModelItem函数扩展
    /// </summary>
    public static class ModelItemExtensions
    {
        internal static bool IsType<T>(this ModelItem modelItem)
        {
            return typeof(T).IsAssignableFrom(modelItem.ItemType);
        }

        public static bool IsFlowNode(this ModelItem modelItem)
        {
            return modelItem.ItemType.BaseType == typeof(FlowNode);
        }

        public static bool IsState(this ModelItem modelItem)
        {
            return modelItem.IsType<State>();
        }

        public static bool IsStateMachine(this ModelItem modelItem)
        {
            return modelItem.ItemType == typeof(StateMachine);
        }

        internal static bool IsActivity(this ModelItem modelItem)
        {
            return modelItem.IsType<Activity>();
        }

        internal static bool IsFlowchart(this ModelItem modelItem)
        {
            return modelItem.ItemType == typeof(Flowchart);
        }

        internal static bool IsFlowDecision(this ModelItem modelItem)
        {
            return modelItem.ItemType == typeof(FlowDecision);
        }

        internal static bool IsFlowStep(this ModelItem modelItem)
        {
            return modelItem.ItemType == typeof(FlowStep);
        }

        internal static bool IsFlowSwitch(this ModelItem modelItem)
        {
            if (modelItem.ItemType.IsGenericType)
            {
                return modelItem.ItemType.GetGenericTypeDefinition() == typeof(FlowSwitch<>);
            }
            return false;
        }

        internal static bool IsPickBranch(this ModelItem modelItem)
        {
            return modelItem.ItemType == typeof(PickBranch);
        }

        internal static bool IsSequence(this ModelItem modelItem)
        {
            return modelItem.ItemType == typeof(Sequence);
        }

        internal static bool IsTransition(this ModelItem modelItem)
        {
            return modelItem.IsType<Transition>();
        }

        internal static bool IsVariable(this ModelItem modelItem)
        {
            return modelItem.IsType<Variable>();
        }

        //是否能添加条目
        public static bool CanAddItem(this ModelItem modelItem, object item)
        {
            if (item == null)
            {
                return false;
            }
            if (item is Activity)
            {
                if (!modelItem.IsSequence())
                {
                    return modelItem.IsFlowchart();
                }
                return true;
            }
            if (item is State || item is FinalState)
            {
                return modelItem.IsStateMachine();
            }
            if (item is FlowNode)
            {
                return modelItem.IsFlowchart();
            }
            return false;
        }

        //添加活动
        public static ModelItem AddActivity(this ModelItem modelItem, object item, int index = -1)
        {
            if (!modelItem.CanAddItem(item))
            {
                return null;
            }
            ModelItem result = null;
            Activity value;
            if (modelItem.IsSequence() && (value = (item as Activity)) != null)
            {
                ModelItemCollection collection = modelItem.Properties["Activities"].Collection;
                result = ((index == -1) ? collection.Add(value) : collection.Insert(index, value));
            }
            else if (modelItem.IsFlowchart())
            {
                FlowNode flowNode = null;
                if (item != null)
                {
                    FlowNode flowNode2;
                    if ((flowNode2 = (item as FlowNode)) == null)
                    {
                        Activity activity;
                        if ((activity = (item as Activity)) != null)
                        {
                            Activity action = activity;
                            flowNode = new FlowStep
                            {
                                Action = action
                            };
                        }
                    }
                    else
                    {
                        FlowNode flowNode3 = flowNode2;
                        flowNode = flowNode3;
                    }
                }
                if (flowNode != null)
                {
                    ModelItemCollection collection2 = modelItem.Properties["Nodes"].Collection;
                    result = ((index == -1) ? collection2.Add(flowNode) : collection2.Insert(index, flowNode));
                }
            }
            else if (modelItem.IsStateMachine() && (item is State || item is FinalState))
            {
                State state = (item as State) ?? new State
                {
                    IsFinal = true,
                };

                ModelItemCollection collection3 = modelItem.Properties["States"].Collection;
                result = ((index == -1) ? collection3.Add(state) : collection3.Insert(index, state));
            }
            return result;
        }

    }
}
