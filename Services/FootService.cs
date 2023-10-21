using Fun.LEGO.Spike;

namespace Walle.Services;


public class FootService(IHubRepl hubRepl, ILogger<FootService> logger) {
    private bool isConnected;
    private readonly SemaphoreSlim connectionLocker = new(1);
    private readonly MotorPair wheel = new(hubRepl, MotorPairs.PAIR_1);

    private async Task Setup() {
        await connectionLocker.WaitAsync();

        if (isConnected) {
            connectionLocker.Release();
            return;
        }

        try {
            await hubRepl.Connect();
            await wheel.Pair(HubPort.A, HubPort.C);
            isConnected = true;
        }
        catch (Exception ex) {
            logger.LogError(ex, "{message}", ex.Message);
            throw;
        }
        finally {
            connectionLocker.Release();
        }
    }

    public async Task Move(double x, double y, double radius) {
        await Setup();

        double leftV, rightV;

        var actualRadius = Math.Sqrt(x * x + y * y);

        var maxV = 1000;
        var mainV = actualRadius / radius * maxV;
        var direction = y > 0 ? 1 : -1;

        if (x == 0) {
            leftV = mainV;
            rightV = mainV;
        }
        else {
            var angle = Math.Atan(y / x);
            var turningRatio = Math.Abs(angle) * 2 / Math.PI;

            if (x > 0) {
                leftV = mainV;
                rightV = mainV * turningRatio;
            }
            else {
                leftV = mainV * turningRatio;
                rightV = mainV;
            }
        }

        await wheel.MoveTank((int)leftV * direction, (int)rightV * direction, acceleration: 10000);
    }

    public async Task StopMove() {
        await wheel.Stop();
    }
}