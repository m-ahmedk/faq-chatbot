using Microsoft.AspNetCore.Mvc.ModelBinding;
using FAQDemo.API.Helpers;

public class CleanStringModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext.ModelType != typeof(string))
            return Task.CompletedTask;

        var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        if (valueProviderResult != ValueProviderResult.None)
        {
            string? value = valueProviderResult.FirstValue;
            value = InputSanitizer.Clean(value);
            bindingContext.Result = ModelBindingResult.Success(value);
        }

        return Task.CompletedTask;
    }
}
