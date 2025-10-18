using MediatR;
using AutoMapper;
using EnergyDashboard.Domain.Entities;
using EnergyDashboard.Domain.Repository;
using EnergyDashboard.Domain.Enums;

namespace EnergyDashboard.Application.LoadFeederTypes.Commands;

public class CreateLoadFeederTypeCommand : IRequest<LoadFeederTypeDto>
{
    public string Name { get; set; }
    public int Type { get; set; }

}

public class CreateLoadFeederTypeCommandHandler : IRequestHandler<CreateLoadFeederTypeCommand, LoadFeederTypeDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILoadFeederTypeRepository _loadFeederTypeRepository;
    private readonly IMapper _mapper;

    public CreateLoadFeederTypeCommandHandler(IUnitOfWork unitOfWork, ILoadFeederTypeRepository loadFeederTypeRepository, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _loadFeederTypeRepository = loadFeederTypeRepository;
        _mapper = mapper;
    }

    public async Task<LoadFeederTypeDto> Handle(CreateLoadFeederTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = new LoadFeederType
        {
            Name = request.Name,
            Type = request.Type,
        };

        var ret = _loadFeederTypeRepository.Add(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return _mapper.Map<LoadFeederTypeDto>(ret);
    }
}
