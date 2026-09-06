namespace Stowaway.Application.Modules.Storage.Container.Commands.Create
{
    public class CreateContainerCommand : IRequest<int>
    {
        public required string Name { get; set; }
        public int ContainerTypeId { get; set; }
        public int WarehouseId { get; set; }
        public int? ParentContainerId { get; set; }
    }
}