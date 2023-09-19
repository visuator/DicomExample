using DicomExample.Models;
using DicomExample.Services;

using Microsoft.AspNetCore.Mvc;

namespace DicomExample.Controllers
{
    [ApiController]
    // Если уж так нужно - то можно создать простой атрибут RoutePrefix, или положить в Area, но последний вариант я не так часто встречал (чисто на основе примеров на гитхабе)
    [Route("api/studies")]
    public class StudiesController : ControllerBase
    {
        private readonly IStudyService _studyService;

        public StudiesController(IStudyService studyService)
        {
            _studyService = studyService;
        }

        [HttpPost()]
        // Валидировать UploadFileModel можно например через FluentValidization или IValidatableObject - например проверить что Stream не пустой.
        public async Task<IActionResult> AddStudy([FromForm]UploadFileModel model, CancellationToken token = default)
        {
            await _studyService.Save(model.File.OpenReadStream(), token);
            return Ok();
        }
    }
}
