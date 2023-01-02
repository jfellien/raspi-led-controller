using Iot.Device.Graphics;
using Iot.Device.Ws28xx;
using System.Device.Spi;
using System.Drawing;

namespace LedStripControllerApi.Services;
internal class LedStrip : ILedStrip
{
    private readonly bool _isInSimulation;
    private int _numberOfLeds;
    private Ws2812b _ledStrip;

    public static LedStrip ForSimulation()
    {
        return new LedStrip();
    }

    public LedStrip(SpiConnectionSettings spiSettings, int numberOfLeds)
    {
        SpiDevice spiDevice = SpiDevice.Create(spiSettings);

        _numberOfLeds = numberOfLeds;

        _ledStrip = new (spiDevice, numberOfLeds);

        _isInSimulation = false;
    }

    protected LedStrip()
    {
        _isInSimulation = true;
    }

    public int NumberOfLeds => _numberOfLeds;

    public void Clear()
    {
        if(_isInSimulation) return;

        BitmapImage image = _ledStrip.Image;
        image.Clear();
        _ledStrip.Update();
    }

    public void TurnOn(Color color)
    {
        if(_isInSimulation) return;

        Clear();

        BitmapImage image = _ledStrip.Image;

        for(int i = 0; i <= _numberOfLeds; i++){
            image.SetPixel(i, 0, color);
        }

        _ledStrip.Update();
    }

    public void TurnOff()
    {
        if(_isInSimulation) return;

        Clear();
    }

    public void Rainbow()
    {
        if(_isInSimulation) return;

        Clear();

        BitmapImage image = _ledStrip.Image;

        for(int j = 0; j < 256; j++)
        {
            for(int i = 0; i < _numberOfLeds; i++){

                image.SetPixel(i,0, Wheel(((i * 256 / _numberOfLeds) + j) % 256));
            }

            _ledStrip.Update();
        }
    }

    public void RainbowDescending()
    {
        if(_isInSimulation) return;

        for(int j = 0; j <= 255; j++){
            Color color = Wheel(j);

            TurnOn(color);
        }
    }

    public void RainbowAscending()
    {
        if(_isInSimulation) return;

        for(int j = 255; j >= 0; j--){
            Color color = Wheel(j);

            TurnOn(color);
        }
    }

    public async Task Strobo(double seconds, CancellationToken cancellation)
    {
        if(_isInSimulation) return;

        DateTimeOffset startTime = DateTimeOffset.UtcNow;
        DateTimeOffset stopTime = startTime.AddSeconds(seconds);
        
        Task stroboTask = new Task(() => 
        {
            while (stopTime > DateTimeOffset.UtcNow) 
            {

                TurnOn(Color.White);

                Thread.Sleep(30);

                TurnOff();

                Thread.Sleep(90);

            }
        });

        await stroboTask;
    }

    public async Task Strobo(double seconds, int onTimeInMilliseconds, int offTimeInMilliseconds, CancellationToken cancellation)
    {
        if(_isInSimulation) return;

        DateTimeOffset startTime = DateTimeOffset.UtcNow;
        DateTimeOffset stopTime = startTime.AddSeconds(seconds);

        Task stroboTask = new Task(() => 
        {
            while (stopTime > DateTimeOffset.UtcNow || cancellation.IsCancellationRequested == false) 
            {

                TurnOn(Color.White);

                Thread.Sleep(onTimeInMilliseconds);

                TurnOff();

                Thread.Sleep(offTimeInMilliseconds);
            }
        });

        await stroboTask.ConfigureAwait(false);
    }

    public async Task RandomStrobo(double seconds, CancellationToken cancellation)
    {
        if(_isInSimulation) return;

        DateTimeOffset startTime = DateTimeOffset.UtcNow;
        DateTimeOffset stopTime = startTime.AddSeconds(seconds);

        Random rnd = new();

        Task stroboTask = new Task(() => 
        {
            while (stopTime > DateTimeOffset.UtcNow) 
            {
                int randomWheel = rnd.Next(0, 255);

                Color randomColor = Wheel(randomWheel);
                
                TurnOn(randomColor);

                Thread.Sleep(30);

                TurnOff();

                Thread.Sleep(90);
            }
        });

        await stroboTask;
    }

    public async Task KnightRider(Color color, int length, int times, CancellationToken cancellation)
    {
        if(_isInSimulation) return;

        Clear();

        Task knightRiderTask = new Task(() =>{

            BitmapImage image = _ledStrip.Image;

            for(int loopCount = 0; loopCount<= times; loopCount++){
                
                if(cancellation.IsCancellationRequested) return;

                for(int i = 0; i <= _numberOfLeds + length; i++)
                {
                    if(i <= _numberOfLeds)
                    {
                        image.SetPixel(i, 0, color);
                    }

                    if(i - length >= 0)
                    {
                        image.SetPixel(i - length, 0, Color.Black);
                    }

                    Thread.Sleep(5);

                    _ledStrip.Update();
                }

                for(int i = _numberOfLeds + length; i >= 0 - length; i--)
            {
                if(i <= _numberOfLeds && i >= 0)
                {
                    image.SetPixel(i, 0, color);
                }

                if(i + length >= 0 && i + length <= _numberOfLeds)
                {
                    image.SetPixel(i + length, 0, Color.Black);
                }

                Thread.Sleep(5);

                _ledStrip.Update();
            }
            
            }
        });

        await knightRiderTask;
    }

    public void RandomColor()
    {
        if(_isInSimulation) return;
        
        Random rnd = new();

        int randomWheel = rnd.Next(0, 255);

        Color randomColor = Wheel(randomWheel);

        TurnOn(randomColor);
    }

    private Color Wheel(int pos)
    {
        if(pos < 85){ 
            return Color.FromArgb(255, pos * 3, 255 - pos * 3, 0);
        }
        else if (pos < 170){

            pos -= 85;
            return Color.FromArgb(255, 255 - pos * 3, 0, pos * 3);
        }
        
        pos -= 170;
        return Color.FromArgb(255, 0, pos * 3, 255 - pos * 3);
    }
}