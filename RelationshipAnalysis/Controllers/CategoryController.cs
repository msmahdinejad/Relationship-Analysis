using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RelationshipAnalysis.Dto.Category;
using RelationshipAnalysis.Services.CategoryServices.EdgeCategory.Abstraction;
using RelationshipAnalysis.Services.CategoryServices.NodeCategory.Abstraction;

namespace RelationshipAnalysis.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]/[action]")]
public class CategoryController(
    ICreateEdgeCategoryService createEdgeCategoryService,
    ICreateNodeCategoryService createNodeCategoryService,
    IEdgeCategoryReceiver edgeCategoryReceiver,
    INodeCategoryReceiver nodeCategoryReceiver
) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllNodeCategories()
    {
        var result = await nodeCategoryReceiver.GetAllNodeCategories();
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllEdgeCategories()
    {
        var result = await edgeCategoryReceiver.GetAllEdgeCategories();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateNodeCategory([FromBody] CreateNodeCategoryDto createNodeCategoryDto)
    {
        var result = await createNodeCategoryService.CreateNodeCategory(createNodeCategoryDto);
        return StatusCode((int)result.StatusCode, result.Data);
    }

    [HttpPost]
    public async Task<IActionResult> CreateEdgeCategory([FromBody] CreateEdgeCategoryDto createEdgeCategoryDto)
    {
        var result = await createEdgeCategoryService.CreateEdgeCategory(createEdgeCategoryDto);
        return StatusCode((int)result.StatusCode, result.Data);
    }
}