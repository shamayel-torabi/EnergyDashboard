using MediatR;
using AutoMapper;
using Application.Common.Exceptions;
using EnergyDashboard.Domain.Entities;
using EnergyDashboard.Domain.Repository;

namespace EnergyDashboard.Application.LoadFeederTypes.Commands;

public class DeleteLoadFeederTypeCommand : IRequest<LoadFeederTypeDto>
{
    public int Id { get; set; }
}

public class DeleteLoadFeederTypeCommandHandler : IRequestHandler<DeleteLoadFeederTypeCommand, LoadFeederTypeDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILoadFeederTypeRepository _loadFeederTypeRepository;
    private readonly IMapper _mapper;

    public DeleteLoadFeederTypeCommandHandler(IUnitOfWork unitOfWork, ILoadFeederTypeRepository loadFeederTypeRepository, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _loadFeederTypeRepository = loadFeederTypeRepository;
        _mapper = mapper;
    }

    public async Task<LoadFeederTypeDto> Handle(DeleteLoadFeederTypeCommand request, CancellationToken cancellationToken)
    {
        var entity = await _loadFeederTypeRepository.GetByIdAsync(request.Id, cancellationToken);

        if (entity is null)
            throw new NotFoundException(nameof(LoadFeederType), request.Id);

        var ret = _loadFeederTypeRepository.Remove(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);       
        return _mapper.Map<LoadFeederTypeDto>(ret);
    }
}
