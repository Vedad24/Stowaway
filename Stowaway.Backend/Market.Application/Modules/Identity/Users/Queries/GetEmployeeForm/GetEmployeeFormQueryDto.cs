namespace Stowaway.Application.Modules.Identity.Users.Queries.GetEmployeeForm
{
    public class GetEmployeeFormQueryDto
    {
        public required List<GetEmployeeFormQuestionDto> Questions { get; set; }
    }

    public class GetEmployeeFormQuestionDto
    {
        public required string Key { get; set; }
        public required string Label { get; set; }
        public bool Required { get; set; }
        public int Order { get; set; }
        public required string ControlType { get; set; }
        public string? Type { get; set; }
        public List<GetEmployeeFormOptionDto>? Options { get; set; }
        public GetEmployeeFormOptionInfoDto? OptionInfo { get; set; }
    }

    public class GetEmployeeFormOptionDto
    {
        public required string Key { get; set; }
        public required string Value { get; set; }
    }

    public class GetEmployeeFormOptionInfoDto
    {
        public required string DisplayName { get; set; }
    }
}
