using Fun.LEGO.Spike;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddRazorComponents().AddInteractiveServerComponents();
services.AddFluentUIComponents(options => {
    options.HostingModel = BlazorHostingModel.Server;
});

services.Configure<HubReplOptions>(options => {
    options.PortName = "/dev/ttyACM0";
});
services.AddSingleton<IHubRepl, HubRepl>();
services.AddSingleton<FootService>();
services.AddSingleton<HeadService>();


var app = builder.Build();


app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<Walle.Views.App>().AddInteractiveServerRenderMode();

app.Run();
