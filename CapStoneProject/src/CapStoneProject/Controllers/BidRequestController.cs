﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CapStoneProject.Repositories;
using CapStoneProject.Repositories.Interfaces;
using CapStoneProject.Models;
using CapStoneProject.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using MimeKit;
using System.Security.Cryptography.X509Certificates;
using Google.Apis.Auth.OAuth2;
using System.Threading;
using MailKit.Net.Smtp;
using MailKit.Security;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CapStoneProject.Controllers
{
    public class BidRequestController : Controller
    {
        private IBidRequestRepo bidReqRepo;
        private IBidRepo bidRepo;
        protected UserManager<UserIdentity> UserManager;
        private CancellationToken taskCancellationToken;


        public BidRequestController(UserManager<UserIdentity> userMgr, IBidRequestRepo repo, IBidRepo bRepo)
        {
            bidReqRepo = repo;
            bidRepo = bRepo;
            UserManager = userMgr;

        }

        [HttpGet]
        public IActionResult BidRequest()
        {
            return View(new BidRequest());
        }

        [HttpPost]
        public async Task<IActionResult> BidRequest(BidRequest bidreq)
        {
            if (!bidReqRepo.UniqueEmail(bidreq.User.Email)) //if false, its not in the database so it is a unique eamil
            {
                if (ModelState.IsValid)
                {

                    bidreq.User.UserName = bidreq.User.Email;
                    bidreq.User.Email = bidreq.User.Email;
                    bidreq.User.FirstName = bidreq.User.FirstName;
                    bidreq.User.LastName = bidreq.User.LastName;

                    IdentityResult result = await UserManager.CreateAsync(bidreq.User, bidreq.User.Password);

                    if (result.Succeeded)
                    {
                        //emailing the client to notify of request
                        var message = new MimeMessage();
                        message.From.Add(new MailboxAddress("Admin", "capstonejoca@yahoo.com"));
                        message.To.Add(new MailboxAddress("Admin", "capstonejoca@yahoo.com"));
                        message.Subject = "bid Request Requested";

                        message.Body = new TextPart("plain")
                        {
                            Text = @"Hey Admin, A new bid request was created please look at it and respond."
                        };

                        using (var client = new SmtpClient())
                        {
                            client.Connect("smtp.mail.yahoo.com", 465, SecureSocketOptions.SslOnConnect);

                            client.Authenticate("capstonejoca@yahoo.com", "JOCA123.");

                            client.Send(message);

                            client.Disconnect(true);
                        }


                        bidReqRepo.Update(bidreq);
                        return RedirectToAction("Success");
                    }
                    else
                    {
                        foreach (IdentityError error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
            }
            return View(bidreq);
        }

        [HttpGet]
        public ViewResult Bid(int bidrequestID)
        {
            BidRequest br = bidReqRepo.GetBidRequestByID(bidrequestID);
            VMBid vmbid = new VMBid();
            vmbid.BidRequestID = bidrequestID;
            vmbid.CustomerFirst = br.User.FirstName;
            vmbid.CustomerLast = br.User.LastName;
            vmbid.ProjectDescription = br.ProjectDescription;
            return View(vmbid);
        }


        [HttpPost]
        public ViewResult Bid(VMBid vmbid)
        {
            if (ModelState.IsValid)
            {
                var br = bidReqRepo.GetBidRequestByID(vmbid.BidRequestID);
                Bid bid = new Models.Bid();
                bid.BidReq = br;
                bid.User = br.User;
                bid.LaborCost = vmbid.LaborCost;
                bid.MaterialsDescription = vmbid.MaterialsDescription;
                bid.RevisedProjectDescription = vmbid.RevisedProjectDescription;
                bid.ProjectedTimeFrame = vmbid.ProjectedTimeFrame;
                bid.ProposedStartDate = bid.ProposedStartDate;
                bid.SupplyCost = vmbid.SupplyCost;
                bid.TotalEstimate = vmbid.TotalEstimate;
                br.Responded = true;
                bidReqRepo.Update(br);
                bidRepo.Update(bid);
                return View("AllBidRequests", bidReqRepo.GetAllBidRequests().ToList());
            }
            else
            {
                ModelState.AddModelError("", "Please fill out all fields in the form.");
            }
            return View(vmbid);
        }




        [HttpGet]
        public ViewResult Success()
        {
            return View();
        }

        [HttpGet]
        public IActionResult LoggedInBidRequest()
        {
            return View(new BidRequest());
        }

        [HttpPost]
        public IActionResult LoggedInBidRequest(BidRequest bidreq)
        {
            //get the user
            var userID = UserManager.GetUserId(HttpContext.User);

            if (ModelState.IsValid)
            {
                bidreq.User.UserName = bidreq.User.Email;
                bidreq.User.Email = bidreq.User.Email;
                bidreq.User.FirstName = bidreq.User.FirstName;
                bidreq.User.LastName = bidreq.User.LastName;

                bidReqRepo.Update(bidreq);
                return RedirectToAction("Success");
            }
            return View(bidreq);
        }

        public IActionResult AllBidRequests()
        {
            return View(bidReqRepo.GetAllBidRequests().ToList());
        }

        public IActionResult AllBids()
        {
            return View(bidRepo.GetAllBids().ToList());
        }

        //TODO: finish and create html
        public ViewResult ViewBid(int bidID)
        {
            return View();
        }

        //TODO: Delete bid and br
        public IActionResult DeleteBidReq(int bidrequestID)
        {
            BidRequest deletedBR = bidReqRepo.DeleteBR(bidrequestID);
            if (deletedBR == null)
            {
                return View("Error");
            }
            return View("AllBidRequests", bidReqRepo.GetAllBidRequests().ToList());           
        }

        public IActionResult DeleteBid(int bidID)
        {
            Bid deletedB = bidRepo.DeleteBid(bidID);
            if (deletedB == null)
            {
                return View("Error");
            }
            return View("AllBids", bidRepo.GetAllBids().ToList());
        }

        
    }
}