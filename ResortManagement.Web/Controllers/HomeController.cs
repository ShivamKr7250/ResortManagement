using Microsoft.AspNetCore.Mvc;
using ResortManagement.Application.Common.Interfaces;
using ResortManagement.Application.Common.Utility;
using ResortManagement.Web.Models;
using ResortManagement.Web.ViewModels;
using Syncfusion.Presentation;
using System.Diagnostics;

namespace ResortManagement.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            HomeVM homeVM = new()
            {
                VillaList = _unitOfWork.Villa.GetAll(includeProperties: "VillaAmenity"),
                Nights = 1,
                CheckInDate = DateOnly.FromDateTime(DateTime.Now),
            };
            return View(homeVM);
        }

        [HttpPost]
        public IActionResult GetVillaByDate(int nights, DateOnly checkInDate)
        {
           // Thread.Sleep(1000);
            var villaList = _unitOfWork.Villa.GetAll(includeProperties: "VillaAmenity");
            var villaNumberList = _unitOfWork.VillaNumber.GetAll().ToList();
            var bookedVillas = _unitOfWork.Booking.GetAll(u => u.Status == SD.StatusApproved || u.Status == SD.StatusCheckedIn).ToList();
            foreach (var villa in villaList)
            {
                int roomAvailable = SD.VillaRoomsAvailable_Count(villa.Id, villaNumberList, checkInDate, nights, bookedVillas);

                villa.IsAvailale = roomAvailable > 0 ? true : false;
            }

            HomeVM homeVM = new()
            { 
                CheckInDate = checkInDate,
                VillaList = villaList,
                Nights = nights
            };

            return PartialView("_VillaList" ,homeVM);
        }

        [HttpPost]
        public IActionResult GeneratePPTExport(int id)
        {
            var villa = _unitOfWork.Villa.GetAll(includeProperties: "VillaAmenity").FirstOrDefault(x => x.Id == id);
            if (villa is null)
            {
                return RedirectToAction(nameof(Error));
            }

            string basePath = _webHostEnvironment.WebRootPath;
            string filePath = basePath + @"/Exports/ExportVillaDetails.pptx";

            using IPresentation presentation = Presentation.Open(filePath);
            ISlide slide = presentation.Slides[0];

            IShape? shape = slide.Shapes.FirstOrDefault(u => u.ShapeName == "txtVillaName") as IShape;
            if (shape is not null)
            {
                shape.TextBody.Text = villa.Name;
            }

            shape = slide.Shapes.FirstOrDefault(u => u.ShapeName == "txtVillaDescription") as IShape;
            if (shape is not null)
            {
                shape.TextBody.Text = villa.Description;
            }

            shape = slide.Shapes.FirstOrDefault(u => u.ShapeName == "txtOccupancy") as IShape;
            if (shape is not null)
            {
                shape.TextBody.Text = string.Format("Max Occupancy : {0} adults", villa.Occupancy);
            }
            shape = slide.Shapes.FirstOrDefault(u => u.ShapeName == "txtVillaSize") as IShape;
            if (shape is not null)
            {
                shape.TextBody.Text = string.Format("Villa Size: {0} sqft", villa.Sqft);
            }
            shape = slide.Shapes.FirstOrDefault(u => u.ShapeName == "txtPricePerNight") as IShape;
            if (shape is not null)
            {
                shape.TextBody.Text = string.Format("IND {0}/night", villa.Price.ToString("c"));
            }

            shape = slide.Shapes.FirstOrDefault(u => u.ShapeName == "txtVillaAmenitiesHeading") as IShape;
            if (shape is not null)
            {
                List<string> listItems = villa.VillaAmenity.Select(u => u.Name).ToList();

                shape.TextBody.Text = "";

                foreach (var item in listItems)
                {
                    IParagraph paragraph = shape.TextBody.AddParagraph();
                    ITextPart textpart = paragraph.AddTextPart(item);

                    paragraph.ListFormat.Type = ListType.Bulleted;
                    paragraph.ListFormat.BulletCharacter = '\u2022';
                    textpart.Font.FontName = "system-ui";
                    textpart.Font.FontSize = 18;
                    textpart.Font.Color = ColorObject.FromArgb(144, 148, 152);
                }
            }

            shape = slide.Shapes.FirstOrDefault(u => u.ShapeName == "imgVilla") as IShape;
            if (shape is not null)
            {
                byte[] imageData;
                string imageUrl;
                try
                {
                    imageUrl = string.Format("{0}{1}", basePath, villa.ImageUrl);
                    imageData = System.IO.File.ReadAllBytes(imageUrl);
                }
                catch
                {
                    imageUrl = string.Format("{0}{1}", basePath, "/images/placeholder.png");
                    imageData = System.IO.File.ReadAllBytes(imageUrl);
                }

                slide.Shapes.Remove(shape);
                using MemoryStream imageStream = new(imageData);
                IPicture newPicture = slide.Pictures.AddPicture(imageStream, 60, 120, 300, 200);
            }


            MemoryStream memmoryStream = new();
            presentation.Save(memmoryStream);
            memmoryStream.Position = 0;
            return File(memmoryStream, "application/pptx", "villa.pptx");

        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
