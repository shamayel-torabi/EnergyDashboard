using MediatR;
using AutoMapper;
using EnergyDashboard.Domain.Entities;
using Application.Common.Exceptions;
using EnergyDashboard.Domain.Repository;
using EnergyDashboard.Domain.Enums;

namespace EnergyDashboard.Application.LoadFeederTypes.Commands;

public class UpdateLoadFeederTypeCommand : IRequest
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Type { get; set; }
}

public class UpdateLoadFeederTypeCommandHandler : IRequestHandler<UpdateLoadFeederTypeCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILoadFeederTypeRepository _loadFeederTypeRepository;
    private readonly IMapper _mapper;

    public UpdateLoadFeederTypeCommandHandler(IUnitOfWork unitOfWork, ILoadFeederTypeRepository loadFeederTypeRepository, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _loadFeederTypeRepository = loadFeederTypeRepository;
        _mapper = mapper;
    }

    public async Task Handle(UpdateLoadFeederTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = await _loadFeederTypeRepository.GetByIdAsync(request.Id, cancellationToken);

        if (entity is null)
            throw new NotFoundException(nameof(LoadFeederType), request.Id);

        entity.Name = request.Name;
        entity.Type = request.Type;

        _loadFeederTypeRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
