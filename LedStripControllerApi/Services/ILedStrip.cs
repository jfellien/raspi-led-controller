using System.Drawing;

namespace LedStripControllerApi.Services;

public interface ILedStrip
{
    void TurnOn(Color color);
    void TurnOff();
}