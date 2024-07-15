namespace Pagination.Helpers
{
    public class SqlCiStringComparer : StringComparer
    {
        public override int Compare(string? x, string? y)
        {
            return string.Compare(x?.Trim(), y?.Trim(), StringComparison.InvariantCultureIgnoreCase);
        }

        public override bool Equals(string? x, string? y)
        {
            return string.Equals(x?.Trim(), y?.Trim(), StringComparison.InvariantCultureIgnoreCase);
        }

        public override int GetHashCode(string obj)
        {
            return obj.Trim().GetHashCode(StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
