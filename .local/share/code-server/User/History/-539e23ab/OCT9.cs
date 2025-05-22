using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Common.Models;
using SchoolManagement.Application.DTOs.Assessment;
using SchoolManagement.Application.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssessmentsController : ControllerBase
    {
        private readonly IAssessmentService _assessmentService;

        public AssessmentsController(IAssessmentService assessmentService)
        {
            _assessmentService = assessmentService;
        }

        /// <summary>
        /// Get all assessments
        /// </summary>
        /// <returns>Collection of assessments</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllAssessments()
        {
            var response = await _assessmentService.GetAllAssessmentsAsync();
            return ToActionResult(response);
        }


        /// <summary>
        /// Get assessment by ID
        /// </summary>
        /// <param name="id">Assessment ID</param>
        /// <returns>Assessment if found</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAssessmentById(string id)
        {
            var response = await _assessmentService.GetAssessmentByIdAsync(id);
            return ToActionResult(response);
        }

        /// <summary>
        /// Get assessments by subject
        /// </summary>
        /// <param name="subjectId">Subject ID</param>
        /// <returns>Collection of assessments for the specified subject</returns>
        [HttpGet("by-subject/{subjectId}")]
        public async Task<IActionResult> GetAssessmentsBySubject(string subjectId)
        {
            var response = await _assessmentService.GetAssessmentsBySubjectAsync(subjectId);
            return ToActionResult(response);
        }

        /// <summary>
        /// Get assessments by section
        /// </summary>
        /// <param name="sectionId">Section ID</param>
        /// <returns>Collection of assessments for the specified section</returns>
        [HttpGet("by-section/{sectionId}")]
        public async Task<IActionResult> GetAssessmentsBySection(string sectionId)
        {
            var response = await _assessmentService.GetAssessmentsBySectionAsync(sectionId);
            return ToActionResult(response);
        }

        /// <summary>
        /// Get assessments by term
        /// </summary>
        /// <param name="termId">Term ID</param>
        /// <returns>Collection of assessments for the specified term</returns>
        [HttpGet("by-term/{termId}")]
        public async Task<IActionResult> GetAssessmentsByTerm(string termId)
        {
            var response = await _assessmentService.GetAssessmentsByTermAsync(termId);
            return ToActionResult(response);
        }

        /// <summary>
        /// Create a new assessment
        /// </summary>
        /// <param name="assessmentDto">Assessment data</param>
        /// <returns>Created assessment</returns>
        [HttpPost]
        public async Task<IActionResult> CreateAssessment(CreateAssessmentDto assessmentDto)
        {
            var response = await _assessmentService.CreateAssessmentAsync(assessmentDto);
            return ToActionResult(response);
        }

        /// <summary>
        /// Update an existing assessment
        /// </summary>
        /// <param name="id">Assessment ID</param>
        /// <param name="assessmentDto">Updated assessment data</param>
        /// <returns>Updated assessment</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAssessment(string id, UpdateAssessmentDto assessmentDto)
        {
            var response = await _assessmentService.UpdateAssessmentAsync(id, assessmentDto);
            return ToActionResult(response);
        }

        /// <summary>
        /// Delete an assessment
        /// </summary>
        /// <param name="id">Assessment ID</param>
        /// <returns>Success or failure response</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAssessment(string id)
        {
            var response = await _assessmentService.DeleteAssessmentAsync(id);
            return ToActionResult(response);
        }

        /// <summary>
        /// Get assessment results by assessment
        /// </summary>
        /// <param name="assessmentId">Assessment ID</param>
        /// <returns>Collection of assessment results for the specified assessment</returns>
        [HttpGet("{assessmentId}/results")]
        public async Task<IActionResult> GetResultsByAssessment(string assessmentId)
        {
            var response = await _assessmentService.GetResultsByAssessmentAsync(assessmentId);
            return ToActionResult(response);
        }

        /// <summary>
        /// Get assessment results by student
        /// </summary>
        /// <param name="studentId">Student ID</param>
        /// <returns>Collection of assessment results for the specified student</returns>
        [HttpGet("results/by-student/{studentId}")]
        public async Task<IActionResult> GetResultsByStudent(string studentId)
        {
            var response = await _assessmentService.GetResultsByStudentAsync(studentId);
            return ToActionResult(response);
        }

        /// <summary>
        /// Record assessment result
        /// </summary>
        /// <param name="resultDto">Assessment result data</param>
        /// <returns>Created assessment result</returns>
        [HttpPost("results")]
        public async Task<IActionResult> RecordResult(CreateAssessmentResultDto resultDto)
        {
            var response = await _assessmentService.RecordResultAsync(resultDto);
            return ToActionResult(response);
        }

        /// <summary>
        /// Update assessment result
        /// </summary>
        /// <param name="id">Assessment result ID</param>
        /// <param name="resultDto">Updated assessment result data</param>
        /// <returns>Updated assessment result</returns>
        [HttpPut("results/{id}")]
        public async Task<IActionResult> UpdateResult(string id, UpdateAssessmentResultDto resultDto)
        {
            var response = await _assessmentService.UpdateResultAsync(id, resultDto);
            return ToActionResult(response);
        }

        /// <summary>
        /// Bulk record assessment results
        /// </summary>
        /// <param name="bulkResultDto">Bulk assessment result data</param>
        /// <returns>Success or failure response</returns>
        [HttpPost("results/bulk")]
        public async Task<IActionResult> BulkRecordResults(BulkAssessmentResultDto bulkResultDto)
        {
            var response = await _assessmentService.BulkRecordResultsAsync(bulkResultDto);
            return ToActionResult(response);
        }

        /// <summary>
        /// Helper method to convert API response to ActionResult
        /// </summary>
        private IActionResult ToActionResult<T>(IApiResponse<T> response)
        {
            if (response.IsSuccess)
            {
                return response.StatusCode switch
                {
                    200 => Ok(response),
                    201 => CreatedAtAction(nameof(GetAssessmentById), new { id = (response.Data as AssessmentDto)?.Id }, response),
                    204 => NoContent(),
                    _ => Ok(response),
                };
            }
            else
            {
                return response.StatusCode switch
                {
                    400 => BadRequest(response),
                    404 => NotFound(response),
                    _ => StatusCode(500, response),
                };
            }
        }
    }
}
