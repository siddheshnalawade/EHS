using AutoMapper;
using EHS.Application.DTOs;
using EHS.Application.Interfaces;
using EHS.Application.Repositories;
using EHS.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace EHS.Infrastructure.Services
{
    public class IncidentNatureService : IIncidentNatureService
    {
        private readonly IRepository<IncidentNature> _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<IncidentNatureService> _logger;

        public IncidentNatureService(
            IRepository<IncidentNature> repository,
            IMapper mapper,
            ILogger<IncidentNatureService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ApiResponse<IncidentNatureResponse>> CreateAsync(CreateIncidentNatureRequest request)
        {
            try
            {
                // Check if incident nature with same name already exists
                var exists = await _repository.ExistsAsync(x => x.Name.ToLower() == request.Name.ToLower());
                if (exists)
                {
                    return new ApiResponse<IncidentNatureResponse>
                    {
                        IsSuccessful = false,
                        Message = "An incident nature with this name already exists."
                    };
                }

                var incidentNature = _mapper.Map<IncidentNature>(request);
                await _repository.AddAsync(incidentNature);
                await _repository.SaveChangesAsync();

                _logger.LogInformation("Incident nature created: {Name}", incidentNature.Name);

                return new ApiResponse<IncidentNatureResponse>
                {
                    IsSuccessful = true,
                    Data = _mapper.Map<IncidentNatureResponse>(incidentNature),
                    Message = "Incident nature created successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating incident nature");
                return new ApiResponse<IncidentNatureResponse>
                {
                    IsSuccessful = false,
                    Message = "An error occurred while creating the incident nature."
                };
            }
        }

        public async Task<ApiResponse<List<IncidentNatureResponse>>> GetAllAsync()
        {
            try
            {
                var incidentNatures = await _repository.GetAllAsync(
                    orderBy: q => q.OrderBy(x => x.Name)
                );

                return new ApiResponse<List<IncidentNatureResponse>>
                {
                    IsSuccessful = true,
                    Data = _mapper.Map<List<IncidentNatureResponse>>(incidentNatures),
                    Message = "Incident natures retrieved successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving incident natures");
                return new ApiResponse<List<IncidentNatureResponse>>
                {
                    IsSuccessful = false,
                    Message = "An error occurred while retrieving incident natures."
                };
            }
        }

        public async Task<ApiResponse<IncidentNatureResponse>> GetByIdAsync(Guid id)
        {
            try
            {
                var incidentNature = await _repository.GetByIdAsync(id);

                if (incidentNature == null)
                {
                    return new ApiResponse<IncidentNatureResponse>
                    {
                        IsSuccessful = false,
                        Message = "Incident nature not found."
                    };
                }

                return new ApiResponse<IncidentNatureResponse>
                {
                    IsSuccessful = true,
                    Data = _mapper.Map<IncidentNatureResponse>(incidentNature),
                    Message = "Incident nature retrieved successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving incident nature {Id}", id);
                return new ApiResponse<IncidentNatureResponse>
                {
                    IsSuccessful = false,
                    Message = "An error occurred while retrieving the incident nature."
                };
            }
        }

        public async Task<ApiResponse<IncidentNatureResponse>> UpdateAsync(Guid id, UpdateIncidentNatureRequest request)
        {
            try
            {
                var incidentNature = await _repository.GetByIdAsync(id);

                if (incidentNature == null)
                {
                    return new ApiResponse<IncidentNatureResponse>
                    {
                        IsSuccessful = false,
                        Message = "Incident nature not found."
                    };
                }

                // Check if another incident nature with same name exists
                var exists = await _repository.ExistsAsync(x =>
                    x.Name.ToLower() == request.Name.ToLower() && x.Id != id);

                if (exists)
                {
                    return new ApiResponse<IncidentNatureResponse>
                    {
                        IsSuccessful = false,
                        Message = "Another incident nature with this name already exists."
                    };
                }

                _mapper.Map(request, incidentNature);
                _repository.UpdateAsync(incidentNature);
                await _repository.SaveChangesAsync();

                _logger.LogInformation("Incident nature updated: {Id}", id);

                return new ApiResponse<IncidentNatureResponse>
                {
                    IsSuccessful = true,
                    Data = _mapper.Map<IncidentNatureResponse>(incidentNature),
                    Message = "Incident nature updated successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating incident nature {Id}", id);
                return new ApiResponse<IncidentNatureResponse>
                {
                    IsSuccessful = false,
                    Message = "An error occurred while updating the incident nature."
                };
            }
        }

        public async Task<ApiResponse<string>> DeleteAsync(Guid id)
        {
            try
            {
                var deleted = await _repository.DeleteAsync(id);

                if (!deleted)
                {
                    return new ApiResponse<string>
                    {
                        IsSuccessful = false,
                        Message = "Incident nature not found."
                    };
                }

                await _repository.SaveChangesAsync();
                _logger.LogInformation("Incident nature deleted: {Id}", id);

                return new ApiResponse<string>
                {
                    IsSuccessful = true,
                    Message = "Incident nature deleted successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting incident nature {Id}", id);
                return new ApiResponse<string>
                {
                    IsSuccessful = false,
                    Message = "An error occurred while deleting the incident nature."
                };
            }
        }
    }
}