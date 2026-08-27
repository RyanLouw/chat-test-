using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;


namespace CallQuality.Utilities;

public static class ControllerRenderExtensions
{
  
    public static string RenderPartialViewToString(this Controller controller, string viewName, object model)
    {
        controller.ViewData.Model = model;

        using var sw = new StringWriter();
        var viewEngine = controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
        var viewResult = viewEngine.FindView(controller.ControllerContext, viewName, false);

        if (!viewResult.Success)
            throw new FileNotFoundException($"View '{viewName}' not found.");

        var viewContext = new ViewContext(
            controller.ControllerContext,
            viewResult.View,
            controller.ViewData,
            controller.TempData,
            sw,
            new HtmlHelperOptions()
        );

        viewResult.View.RenderAsync(viewContext).Wait();
        return sw.ToString();
    }
}
