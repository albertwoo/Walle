namespace Walle.Services;

using System.Device.Pwm;
using System.Device.I2c;
using Iot.Device.Pwm;
using Iot.Device.ServoMotor;

public class HeadService : IDisposable {
    const int topPIN = 5;
    const int bottomPIN = 4;

    readonly I2cConnectionSettings settings;
    readonly I2cDevice device;
    readonly Pca9685 pca9685;
    readonly ServoMotor topMotor;
    readonly ServoMotor bottomMotor;

    public HeadService() {
        settings = new(1, 64);
        device = I2cDevice.Create(settings);
        pca9685 = new Pca9685(device, pwmFrequency: 60);
        topMotor = new ServoMotor(pca9685.CreatePwmChannel(topPIN));
        bottomMotor = new ServoMotor(pca9685.CreatePwmChannel(bottomPIN));

        topMotor.Start();
        bottomMotor.Start();
    }

    public void Dispose() {
        device.Dispose();
        pca9685.Dispose();
        topMotor.Stop();
        topMotor.Dispose();
        bottomMotor.Stop();
        bottomMotor.Dispose();
    }

    public Task Move(double x, double y, double radius) {
        bottomMotor.WriteAngle(90 + x / radius * 90);
        topMotor.WriteAngle(90 + y / radius * 90);
        return Task.CompletedTask;
    }
}