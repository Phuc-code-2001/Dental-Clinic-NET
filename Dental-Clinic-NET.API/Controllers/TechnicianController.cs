﻿using DataLayer.Domain;
using Dental_Clinic_NET.API.DTOs;
using Dental_Clinic_NET.API.Models.Appointments;
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

        [HttpPost]
        [Authorize(Roles = nameof(UserType.Technician))]
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

                    //SegmentationResultDTO jsonResult = _servicesManager.AutoMapper.Map<SegmentationResultDTO>(result);

                    //return Ok(jsonResult);
                    return Ok("Upload succeeded.");
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
                
                List<SegmentationResult> results = _servicesManager.DbContext.SegmentationResults
                    .Include(x => x.Appointment)
                    .Include(x => x.Technican)
                    .Include(x => x.ImageResultSet)
                    .Where(x => x.Appointment.Id == appointmentId).ToList();

                var jsonResults = _servicesManager.AutoMapper.Map<SegmentationResultDTOLite[]>(results.ToArray());

                return Ok(jsonResults);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Authorize(Roles = nameof(UserType.Technician))]
        public IActionResult GetAppointmentQueue([FromQuery] AppointmentFilter filter)
        {
            try
            {

                var validStates = new HashSet<Appointment.States>
                {
                    Appointment.States.Transfer,
                    Appointment.States.TransferDoing,
                    Appointment.States.TransferCancel,
                    Appointment.States.TransferComplete
                };

                IQueryable<Appointment> query = _servicesManager.DbContext.Appointments
                    .Include(x => x.Patient.BaseUser)
                    .Include(x => x.Doctor.BaseUser)
                    .Include(x => x.Service)
                    .Where(x => validStates.Contains(x.State));

                query = filter.Filter(query).OrderByDescending(x => x.Date);

                var paginated = new Paginated<Appointment>(query, filter.Page, filter.PageSize);
                var jsonData = paginated.GetData((items) =>
                _servicesManager.AutoMapper.Map<Appointment[], AppointmentDTOLite[]>(items.ToArray()));

                return Ok(jsonData);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete]
        public IActionResult RemoveSegmentationResult(int id)
        {
            try
            {
                SegmentationResult obj = _servicesManager.DbContext.SegmentationResults
                    .FirstOrDefault(x => x.Id == id);

                if (obj == null) return NotFound();

                _servicesManager.DbContext.Remove(obj);
                _servicesManager.DbContext.SaveChanges();

                return Ok();

            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
