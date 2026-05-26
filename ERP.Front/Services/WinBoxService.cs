using Microsoft.JSInterop;

namespace ERP.Front.Services;

public class WinBoxService
{
    private readonly IJSRuntime jsRuntime;
    private readonly Dictionary<string, WindowInfo> windows = new();
    private string? activeWindowId;
    private int nextZIndex = 100;

    public event Action? OnWindowsChanged;
    public event Action<string>? OnWindowFocusChanged;
    public event Action<string>? OnWindowClosed;

    public WinBoxService(IJSRuntime jsRuntime)
    {
        this.jsRuntime = jsRuntime;
    }

    public IReadOnlyDictionary<string, WindowInfo> Windows => windows;
    public string? ActiveWindowId => activeWindowId;

    public async Task OpenWindow(string id, string title, string url, int? width = null, int? height = null)
    {
        if (windows.TryGetValue(id, out var existingWindow))
        {
            if (existingWindow.IsMinimized)
            {
                await FocusWindow(id);
            }
            else if (existingWindow.IsOpen)
            {
                await FocusWindow(id);
            }
            return;
        }

        int windowWidth = width ?? 1000;
        int windowHeight = height ?? 700;

        int centerX = (WindowWidth - windowWidth) / 2;
        int centerY = (WindowHeight - windowHeight) / 2;

        bool isExternal = url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || 
                          url.StartsWith("https://", StringComparison.OrdinalIgnoreCase);

        windows[id] = new WindowInfo
        {
            Id = id,
            Title = title,
            Url = url,
            IsExternal = isExternal,
            IsOpen = true,
            IsMinimized = false,
            IsFocused = true,
            Width = windowWidth,
            Height = windowHeight,
            X = Math.Max(50, centerX),
            Y = Math.Max(50, centerY),
            ZIndex = nextZIndex++
        };

        activeWindowId = id;
        
        foreach (var w in windows.Values.Where(w => w.Id != id))
        {
            w.IsFocused = false;
        }

        OnWindowsChanged?.Invoke();
    }

    public async Task CloseWindow(string id)
    {
        windows.Remove(id);
        
        if (activeWindowId == id)
        {
            var lastWindow = windows.Values.LastOrDefault();
            activeWindowId = lastWindow?.Id;
            if (lastWindow != null)
            {
                lastWindow.IsFocused = true;
            }
        }
        
        OnWindowsChanged?.Invoke();
        OnWindowClosed?.Invoke(id);
    }

    public async Task MinimizeWindow(string id)
    {
        if (windows.TryGetValue(id, out var window))
        {
            window.IsMinimized = true;
            window.IsFocused = false;
            
            var nextActive = windows.Values.Where(w => w.IsOpen && !w.IsMinimized).LastOrDefault();
            if (nextActive != null)
            {
                activeWindowId = nextActive.Id;
                nextActive.IsFocused = true;
            }
            else
            {
                activeWindowId = null;
            }
            
            OnWindowsChanged?.Invoke();
        }
    }

    public async Task MaximizeWindow(string id)
    {
        if (windows.TryGetValue(id, out var window))
        {
            if (window.IsMaximized)
            {
                window.X = window.RestoreX;
                window.Y = window.RestoreY;
                window.Width = window.RestoreWidth;
                window.Height = window.RestoreHeight;
                window.IsMaximized = false;
            }
            else
            {
                window.RestoreX = window.X;
                window.RestoreY = window.Y;
                window.RestoreWidth = window.Width;
                window.RestoreHeight = window.Height;
                window.X = 0;
                window.Y = 0;
                window.Width = WindowWidth;
                window.Height = WindowHeight - 48;
                window.IsMaximized = true;
            }
            
            await FocusWindow(id);
        }
    }

    public async Task FocusWindow(string id)
    {
        if (windows.TryGetValue(id, out var window))
        {
            window.IsMinimized = false;
            window.IsFocused = true;
            window.ZIndex = nextZIndex++;
            activeWindowId = id;
            
            foreach (var w in windows.Values.Where(w => w.Id != id))
            {
                w.IsFocused = false;
            }
            
            OnWindowFocusChanged?.Invoke(id);
            OnWindowsChanged?.Invoke();
        }
    }

    public void UpdateWindowPosition(string id, int x, int y)
    {
        if (windows.TryGetValue(id, out var window))
        {
            window.X = x;
            window.Y = y;
            OnWindowsChanged?.Invoke();
        }
    }

    public void UpdateWindowSize(string id, int width, int height)
    {
        if (windows.TryGetValue(id, out var window))
        {
            window.Width = Math.Max(400, width);
            window.Height = Math.Max(300, height);
            OnWindowsChanged?.Invoke();
        }
    }

    public async Task BringToFront(string id)
    {
        await FocusWindow(id);
    }

    private int WindowWidth => 1920;
    private int WindowHeight => 1080;
}

public class WindowInfo
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public bool IsExternal { get; set; }
    public bool IsOpen { get; set; }
    public bool IsMinimized { get; set; }
    public bool IsFocused { get; set; }
    public bool IsMaximized { get; set; }
    public int Width { get; set; } = 1000;
    public int Height { get; set; } = 700;
    public int X { get; set; } = 100;
    public int Y { get; set; } = 100;
    public int ZIndex { get; set; } = 100;
    public int RestoreX { get; set; }
    public int RestoreY { get; set; }
    public int RestoreWidth { get; set; }
    public int RestoreHeight { get; set; }
}