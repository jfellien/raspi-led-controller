using System.Drawing;

namespace LedStripControllerApi.Services;

public interface ILedStrip
{
    int NumberOfLeds{ get; }
    void TurnOn(Color color);
    void TurnOff();
    void Rainbow();
    void RainbowAscending();
    void RainbowDescending();
    void RandomColor();
    Task Strobo(double seconds, CancellationToken cancellation);
    Task Strobo(double seconds, int onTimeInMilliseconds, int offTimeInMilliseconds, CancellationToken cancellation);
    Task RandomStrobo(double seconds, CancellationToken cancellation);
    Task KnightRider(Color color, int length, int times, CancellationToken cancellation);
}