using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

namespace CustomComponentExample;

// Blazor components are used in the CMS content with their class name as the tag name:
//   <CustomComponent Parameter="test" />
//   <CustomComponent>Hello World</CustomComponent>
[Description("My Custom Component")]
public sealed class CustomComponent : ComponentBase
{
    // Services are injected with the Inject attribute, not through the constructor
    [Inject] private ILogger<CustomComponent> Logger { get; set; } = null!;

    // Attributes of the tag
    [Parameter, Description("Custom Parameter")] public string? Parameter { get; set; }

    // The inner HTML of the tag
    [Parameter] public RenderFragment? ChildContent { get; set; }

    // All other attributes of the tag
    [Parameter(CaptureUnmatchedValues = true)] public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    protected override void OnInitialized()
    {
        Logger.LogInformation("Custom Component initialized");
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "div");
        builder.AddMultipleAttributes(1, AdditionalAttributes);

        if (Parameter is not null)
        {
            builder.AddContent(2, $"Parameter={Parameter}");
        }
        if (ChildContent is not null)
        {
            builder.AddContent(3, ChildContent);
        }

        builder.CloseElement();
    }
}
