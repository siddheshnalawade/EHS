namespace EHS.Application.Constants
{
    /// <summary>
    /// Constants for incident status workflow.
    /// These should match the seeded data in the database.
    /// </summary>
    public static class IncidentStatusConstants
    {
        public const string Submitted = "Submitted";
        public const string Assigned = "Assigned";
        public const string Approved = "Approved";
        public const string InProgress = "InProgress";
        public const string VerificationPending = "VerificationPending";
        public const string Closed = "Closed";
        public const string Rejected = "Rejected";
    }
}