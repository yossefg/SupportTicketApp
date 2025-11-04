namespace CustomerSuppTicket.API.EndPoints
{
    using AutoMapper;
    using CustomerSuppTicket.Common.DTOs;
    using CustomerSuppTicket.Common.ViewModels;
    using CustomerSuppTicket.Common.Intefaces.Services;

    public static class TicketEndpoints
    {
        public static void MapTicketEndpoints(this WebApplication app)
        {
            app.MapPost("/api/tickets", async (TicketViewModel vm, ITicketService service, IMapper mapper) =>
            {
                var dto = mapper.Map<TicketDto>(vm);
                var created = await service.CreateAsync(dto);

                return Results.Created($"/api/tickets/{created.Id}", created);
            });

            app.MapGet("/api/tickets", async (ITicketService service) =>
                await service.GetAllAsync())
                .RequireAuthorization();

            app.MapGet("/api/tickets/{id:guid}", async (Guid id, ITicketService service) =>
                await service.GetByIdAsync(id) is TicketDto t ? Results.Ok(t) : Results.NotFound());

            app.MapPut("/api/tickets", async (TicketUpdateStatusViewModel vm, ITicketService service) =>
            {
                var ticket = await service.UpdateStatusAsync(vm);
                return ticket is null ? Results.NotFound() : Results.Ok(ticket);
            }).RequireAuthorization();

            app.MapPut("/api/tickets/bulk-update", async (
                List<TicketUpdateStatusViewModel> list,
                ITicketService service) =>
            {
                if (list == null || list.Count == 0)
                    return Results.BadRequest("No tickets received.");

                var results = await service.UpdateStatusBulkAsync(list);
                return Results.Ok(results);
            }).RequireAuthorization();
        }
    }

}
