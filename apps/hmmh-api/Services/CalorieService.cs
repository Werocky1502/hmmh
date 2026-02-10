using Hmmh.Api.Contracts.Requests;
using Hmmh.Api.Contracts.Responses;
using Hmmh.Api.Exceptions;
using Hmmh.Api.Factories;
using Hmmh.Api.Db.Models;
using Hmmh.Api.Db.Repositories;
using Hmmh.Api.Extensions;

namespace Hmmh.Api.Services;

/// <summary>
///     Handles calorie tracking workflows for the API.
/// </summary>
public sealed class CalorieService : ICalorieService
{
    private readonly IRepository<CalorieEntry> calorieRepository;
    private readonly ICalorieEntryFactory calorieFactory;
    private readonly ICalorieEntryResponseFactory responseFactory;
    private readonly ILogger<CalorieService> logger;

    /// <summary>
    ///     Initializes a new instance of the <see cref="CalorieService" /> class.
    /// </summary>
    /// <param name="calorieRepository">Repository for calorie data.</param>
    /// <param name="calorieFactory">Factory for calorie entries.</param>
    /// <param name="logger">Logger for calorie operations.</param>
    public CalorieService(
        IRepository<CalorieEntry> calorieRepository,
        ICalorieEntryFactory calorieFactory,
        ICalorieEntryResponseFactory responseFactory,
        ILogger<CalorieService> logger)
    {
        this.calorieRepository = calorieRepository;
        this.calorieFactory = calorieFactory;
        this.responseFactory = responseFactory;
        this.logger = logger;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<CalorieEntryResponse>> GetCaloriesByDateAsync(
        Guid userId,
        DateOnly date,
        CancellationToken cancellationToken)
    {
        // Load and map calorie entries for the supplied date.
        var entries = await calorieRepository.ListAsync(
            entry => entry.UserId == userId && entry.EntryDate == date,
            false,
            query => query.OrderBy(entry => entry.Id),
            cancellationToken);
        var response = entries.Select(responseFactory.Create).ToList();
        return response;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<CalorieEntryResponse>> GetCaloriesAsync(
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

        var entries = await calorieRepository.ListAsync(
            entry => entry.UserId == userId && entry.EntryDate >= startDate && entry.EntryDate <= endDate,
            false,
            query => query.OrderBy(entry => entry.EntryDate).ThenBy(entry => entry.Id),
            cancellationToken);
        var response = entries.Select(responseFactory.Create).ToList();
        return response;
    }

    /// <inheritdoc />
    public async Task<CalorieEntryResponse> CreateCalorieAsync(
        Guid userId,
        CalorieEntryRequest request,
        CancellationToken cancellationToken)
    {
        // Create and persist the new calorie entry.
        var entry = calorieFactory.Create(
            userId,
            request.Date,
            request.Calories,
            request.FoodName.NormalizeOptionalText(),
            request.PartOfDay.NormalizeOptionalText(),
            request.Note.NormalizeOptionalText());

        await calorieRepository.AddAsync(entry, cancellationToken);
        await calorieRepository.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Saved calorie entry for user {UserId} on {Date}.", userId, request.Date);
        return responseFactory.Create(entry);
    }

    /// <inheritdoc />
    public async Task DeleteCalorieAsync(Guid userId, Guid entryId, CancellationToken cancellationToken)
    {
        // Remove the calorie entry if it belongs to the current user.
        var entry = await calorieRepository.FindAsync(
            calorieEntry => calorieEntry.UserId == userId && calorieEntry.Id == entryId,
            true,
            cancellationToken);
        if (entry is null)
        {
            throw new NotFoundApiException("Calorie entry was not found.");
        }

        calorieRepository.Remove(entry);
        await calorieRepository.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Deleted calorie entry {EntryId} for user {UserId}.", entryId, userId);
    }

}
