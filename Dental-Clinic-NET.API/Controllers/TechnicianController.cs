using DataLayer.Domain;
using Dental_Clinic_NET.API.DTOs;
using Dental_Clinic_NET.API.Models.Technician;
using Dental_Clinic_NET.API.Services;
using Dental_Clinic_NET.API.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SegementationXRayServices;
using SegementationXRayServices.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static DataLayer.Domain.SegmentationResult;

namespace Dental_Clinic_NET.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TechnicianController : ControllerBase
    {
        ServicesManager _servicesManager;
        XRayClient _client;

        public TechnicianController(ServicesManager servicesManager, XRayClient client)
        {
            _servicesManager = servicesManager;
            _client = client;
        }


        [HttpGet]
        public IActionResult GetAppointmentQueue([FromQuery] PageFilter filter)
        {
            try
            {
                var queries = _servicesManager.DbContext.Appointments
                    .Where(x => x.State == Appointment.States.Transfer
                    || x.State == Appointment.States.TransferDoing
                    || x.State == Appointment.States.TransferCancel
                    || x.State == Appointment.States.TransferComplete)
                    .OrderBy(x => x.State);

                Paginated<Appointment> paginated = new Paginated<Appointment>(queries, filter.Page, filter.PageSize);

                var dataset = paginated.GetData(items =>
                {
                    return _servicesManager.AutoMapper.Map<AppointmentDTOLite[]>(items.ToArray());
                });

                return Ok(dataset);

            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPost]
        [Authorize(Roles = nameof(UserType.Technican))]
        public async Task<IActionResult> UploadXRayImageAsync([FromForm] UploadXRayForm form)
        {
            try
            {

                BaseUser technicican = _servicesManager.UserServices.GetLoggedUser(HttpContext);

                // Find Appointment
                Appointment appointment = await _servicesManager.DbContext.Appointments
                                .Include(x => x.SegmentationResults)
                                .FirstOrDefaultAsync(x => x.Id == form.AppointmentId);

                if(appointment == null)
                {
                    return NotFound("Appointment not found!");
                }

                // Do some thing
                string purpose = $"ShinyTeeth System: {appointment.Id}";
                var requestData = new PredictionRequest(form.Image, purpose);

                var responseData = await _client.UploadFileAsync(requestData);
                if(requestData != null)
                {

                    SegmentationResult result = new SegmentationResult()
                    {
                        Appointment = appointment,
                        Technican = technicican,
                        ModelName = responseData.ModuleName,
                        ImageResultSet = responseData.PredictionResultSet.First().ImageResultSet
                            .Select(x => new SegmentationImageResult()
                            {
                                Title = x.Title,
                                ImageURL = x.Image,
                            }).ToList(),
                        TeethCount = responseData.PredictionResultSet.First().TeethCount,
                        InputImageURL = responseData.InputImage
                    };

                    _servicesManager.DbContext.SegmentationResults.Add(result);
                    _servicesManager.DbContext.SaveChanges();

                    SegmentationResultDTO jsonResult = _servicesManager.AutoMapper.Map<SegmentationResultDTO>(result);

                    return Ok(jsonResult);
                }


                throw new Exception("Something wrong!");

            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public IActionResult GetSegmentationResults(int appointmentId)
        {
            try
            {
                Appointment appointment = _servicesManager.DbContext.Appointments
                    .Include(x => x.SegmentationResults)
                    .ThenInclude(x => x.ImageResultSet)
                    .FirstOrDefault(x => x.Id == appointmentId);

                if (appointment == null)
                {
                    return NotFound("Appoiment Not Found!");
                }

                List<SegmentationResult> results = appointment.SegmentationResults;

                var jsonResults = _servicesManager.AutoMapper.Map<SegmentationResultDTOLite[]>(results.ToArray());

                return Ok(jsonResults);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
