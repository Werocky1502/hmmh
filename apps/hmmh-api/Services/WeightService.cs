using Hmmh.Api.Contracts.Requests;
using Hmmh.Api.Contracts.Responses;
using Hmmh.Api.Exceptions;
using Hmmh.Api.Factories;
using Hmmh.Api.Db.Models;
using Hmmh.Api.Db.Repositories;

namespace Hmmh.Api.Services;

/// <summary>
///     Handles weight tracking workflows for the API.
/// </summary>
public sealed class WeightService : IWeightService
{
    private readonly IRepository<WeightEntry> weightRepository;
    private readonly IWeightEntryFactory weightFactory;
    private readonly IWeightEntryResponseFactory responseFactory;
    private readonly ILogger<WeightService> logger;

    /// <summary>
    ///     Initializes a new instance of the <see cref="WeightService" /> class.
    /// </summary>
    /// <param name="weightRepository">Repository for weight data.</param>
    /// <param name="weightFactory">Factory for weight entries.</param>
    /// <param name="logger">Logger for weight operations.</param>
    public WeightService(
        IRepository<WeightEntry> weightRepository,
        IWeightEntryFactory weightFactory,
        IWeightEntryResponseFactory responseFactory,
        ILogger<WeightService> logger)
    {
        this.weightRepository = weightRepository;
        this.weightFactory = weightFactory;
        this.responseFactory = responseFactory;
        this.logger = logger;
    }

    /// <inheritdoc />
    public async Task<WeightEntryResponse> GetWeightByDateAsync(
        Guid userId,
        DateOnly date,
        CancellationToken cancellationToken)
    {
        // Return the requested weight entry or an empty placeholder.
        var entry = await weightRepository.FindAsync(
            weight => weight.UserId == userId && weight.EntryDate == date,
            false,
            cancellationToken);
        return entry is null
            ? responseFactory.CreateEmpty(date)
            : responseFactory.Create(entry);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<WeightEntryResponse>> GetWeightsAsync(
        Guid userId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken)
    {
        // Validate the date range before loading data.
        if (endDate < startDate)
        {
            throw new ValidationApiException("End date must be on or after the start date.");
        }

        var entries = await weightRepository.ListAsync(
            weight => weight.UserId == userId && weight.EntryDate >= startDate && weight.EntryDate <= endDate,
            false,
            query => query.OrderBy(weight => weight.EntryDate),
            cancellationToken);
        var response = entries.Select(responseFactory.Create).ToList();
        return response;
    }

    /// <inheritdoc />
    public async Task<WeightEntryResponse> UpsertWeightAsync(
        Guid userId,
        WeightEntryRequest request,
        CancellationToken cancellationToken)
    {
        // Create or update the weight entry for the supplied date.
        var entry = await weightRepository.FindAsync(
            weight => weight.UserId == userId && weight.EntryDate == request.Date,
            true,
            cancellationToken);
        if (entry is null)
        {
            entry = weightFactory.Create(userId, request.Date, request.WeightKg);
            await weightRepository.AddAsync(entry, cancellationToken);
        }
        else
        {
            entry.WeightKg = request.WeightKg;
        }

        await weightRepository.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Saved weight entry for user {UserId} on {Date}.", userId, request.Date);

        return responseFactory.Create(entry);
    }

    /// <inheritdoc />
    public async Task DeleteWeightAsync(Guid userId, Guid entryId, CancellationToken cancellationToken)
    {
        // Remove the weight entry if it belongs to the current user.
        var entry = await weightRepository.FindAsync(
            weight => weight.UserId == userId && weight.Id == entryId,
            true,
            cancellationToken);
        if (entry is null)
        {
            throw new NotFoundApiException("Weight entry was not found.");
        }

        weightRepository.Remove(entry);
        await weightRepository.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Deleted weight entry {EntryId} for user {UserId}.", entryId, userId);
    }

}
