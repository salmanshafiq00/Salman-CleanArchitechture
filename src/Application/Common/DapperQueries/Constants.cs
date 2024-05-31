namespace CleanArchitechture.Application.Common.DapperQueries;

public static class Constants
{
    public static class TField
    {
        public const string TString = "string";
        public const string TBool = "bool";
        public const string TSelect = "select";
        public const string TMultiSelect = "multiselect";
        public const string TDate = "date";
        public const string TDateTime = "datetime";
        public const string TDateRange = "daterange";
    }

    public static class MatchMode
    {
        public const string StartsWith = "startsWith";
        public const string EndsWith = "endsWith";
        public const string Contains = "contains";
        public const string NotContains = "notContains";
        public const string Equality = "equals";
        public const string NotEquals = "notEquals";
    }
}

public static class SqlConstants
{
    public static class S
    {
        public const string LIKE = nameof(LIKE);
        public const string AND = nameof(AND);
        public const string OR = nameof(OR);
        public const string EQUAL = nameof(EQUAL);

        public const string SELECT = nameof(SELECT);
        public const string FROM = nameof(FROM);
        public const string WHERE = nameof(WHERE);
        public const string ORDERBY = "ORDER BY";
        public const string GROUPBY = "GROUP BY";
        public const string COUNT = nameof(COUNT);
        public const string HAVING = nameof(HAVING);
        public const string INSERT = nameof(INSERT);
        public const string DELETE = nameof(DELETE);
        public const string UPDATE = nameof(UPDATE);
        public const string TOP = nameof(TOP);
        public const string LEFT = nameof(LEFT);
        public const string RIGHT = nameof(RIGHT);
        public const string NULL = nameof(NULL);
        public const string JOIN = nameof(JOIN);
        public const string L_JOIN = "LEFT JOIN";
        public const string R_JOIN = "RIGHT JOIN";
        public const string ASC = nameof(ASC);
        public const string DESC = nameof(DESC);
        public const string OVER = nameof(OVER);
        public const string IN = nameof(IN);
        public const string O_JOIN = "OUT JOIN";
        public const string UPPER = nameof(UPPER);
        public const string LOWER = nameof(LOWER);
    }
}
