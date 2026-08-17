using Stowaway.Domain.Entities.Identity;

namespace Stowaway.Application.Modules.Identity.Users.Queries.GetEmployeeForm
{
    public class GetEmployeeFormQueryHandler : IRequestHandler<GetEmployeeFormQuery, GetEmployeeFormQueryDto>
    {
        public Task<GetEmployeeFormQueryDto> Handle(GetEmployeeFormQuery request, CancellationToken cancellationToken)
        {
            GetEmployeeFormQueryDto dto = new()
            {
                Questions =
                [
                    new GetEmployeeFormQuestionDto
                    {
                        Key = "email",
                        Label = "Email",
                        Required = true,
                        Order = 1,
                        ControlType = "textbox",
                        Type = "email",
                    },
                    new GetEmployeeFormQuestionDto
                    {
                        Key = "firstName",
                        Label = "First Name",
                        Required = true,
                        Order = 2,
                        ControlType = "textbox",
                        Type = "text",
                    },
                    new GetEmployeeFormQuestionDto
                    {
                        Key = "lastName",
                        Label = "Last Name",
                        Required = true,
                        Order = 3,
                        ControlType = "textbox",
                        Type = "text",
                    },
                    new GetEmployeeFormQuestionDto
                    {
                        Key = "password",
                        Label = "Password",
                        Required = true,
                        Order = 4,
                        ControlType = "textbox",
                        Type = "password",
                    },
                    new GetEmployeeFormQuestionDto
                    {
                        Key = "role",
                        Label = "Role",
                        Required = true,
                        Order = 5,
                        ControlType = "autocomplete",
                        Options =
                        [
                            new GetEmployeeFormOptionDto { Key = ((int)Role.User).ToString(), Value = "Employee" },
                            new GetEmployeeFormOptionDto { Key = ((int)Role.Manager).ToString(), Value = "Manager" },
                        ],
                        OptionInfo = new GetEmployeeFormOptionInfoDto { DisplayName = "value" },
                    },
                ],
            };

            return Task.FromResult(dto);
        }
    }
}
