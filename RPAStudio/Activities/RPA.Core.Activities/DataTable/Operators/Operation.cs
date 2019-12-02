using System.ComponentModel.DataAnnotations;

namespace RPA.Core.Activities.DataTableActivity
{
    public enum FilterOperator
    {
        [Display(Name = "<")]
        LT,
        [Display(Name = ">")]
        GT,
        [Display(Name = "<=")]
        LTE,
        [Display(Name = ">=")]
        GTE,
        [Display(Name = "=")]
        EQ,
        [Display(Name = "!=")]
        NOTEQ,
        [Display(Name = "Is Empty")]
        EMPTY,
        [Display(Name = "Is Not Empty")]
        NOTEMPTY,
        [Display(Name = "Starts With")]
        STARTSWITH,
        [Display(Name = "Ends With")]
        ENDSWITH,
        [Display(Name = "Contains")]
        CONTAINS,
        [Display(Name = "Does Not Start With")]
        NOTSTARTSWITH,
        [Display(Name = "Does Not End With")]
        NOTENDSWITH,
        [Display(Name = "Does Not Contain")]
        NOTCONTAINS
    }

    public enum JoinOperator
    {
        [Display(Name = "<")]
        LT,
        [Display(Name = ">")]
        GT,
        [Display(Name = "<=")]
        LTE,
        [Display(Name = ">=")]
        GTE,
        [Display(Name = "=")]
        EQ,
        [Display(Name = "!=")]
        NOTEQ
    }

    public enum SelectMode
    {
        Keep,
        Remove
    }
    public enum JoinType
    {
        Inner,
        Left,
        Full
    }
}
