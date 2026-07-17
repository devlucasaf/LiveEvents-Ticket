namespace LiveEventsTicket.Backend.Modules.Ingresso.Dto;

public class ModalidadesDto
{
    public decimal              MeiaFator       { get; set; }
    public decimal              SocialAcrescimo { get; set; }
    public List<MeiaSubtipoDto> MeiaSubtipos    { get; set; } = new();
}
