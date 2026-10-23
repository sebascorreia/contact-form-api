namespace ContactAPI.Validation
{
    public static class FieldErrors
    {
        private static readonly Dictionary<string, string> Messages = new()
        {
            ["firstName"] = "Please enter a valid first name",
            ["lastName"] = "Please enter a valid last name",
            ["email"] = "Please enter a valid email address",
            ["phone"] = "Please enter a valid phone number",
            ["country"] = "Please enter a valid country",
            ["message"] = "Please enter a message (1-5000 characters)",
            ["roles"] = "Please select at least one valid role",
            ["source"] = "Invalid form source",
        };
        public static string ToFieldName(string key)
        {
            var name = key.Split().Last().TrimStart('$');
            return name.Length == 0 ? "body" : char.ToLowerInvariant(name[0]) + name[1..];
        }

        public static string MessageFor(string field) =>
            Messages.TryGetValue(field, out var message) ? message : "Invalid Input";
    }
}
