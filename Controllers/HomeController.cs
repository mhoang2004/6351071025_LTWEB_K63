using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplicationTH.Models;

namespace WebApplicationTH.Controllers
{
    public class HomeController : Controller
    {
        private QLBansachEntities db = new QLBansachEntities();
        public ActionResult Index()
        {
            var books = db.SACHes
                  .OrderByDescending(b => b.Ngaycapnhat)
                  .Take(5)
                  .ToList();

            var distinctSubjects = db.CHUDEs.Select(cd => cd).Distinct().ToList();

            var distinctPublishers = db.NHAXUATBANs.Select(nxb => nxb).Distinct().ToList();

            var viewModel = new HomeViewModel
            {
                Books = books,
                DistinctSubjects = distinctSubjects,
                DistinctPublishers = distinctPublishers
            };

            return View(viewModel);
        }

        public ActionResult Detail(int id)
        {
            var book = db.SACHes.Find(id);
            if (book == null)
            {
                return HttpNotFound();
            }

            return View(book); // Assuming you have a corresponding Detail view for displaying the book details
        }

        public ActionResult Subjects(int id)
        {
            // Retrieve the subject based on the provided id
            var subject = db.CHUDEs.Find(id);
            if (subject == null)
            {
                return HttpNotFound(); // Return a 404 error if the subject is not found
            }

            // Retrieve all books with the same subject id (MaCD)
            var books = db.SACHes.Where(b => b.MaCD == id).ToList();


            var distinctSubjects = db.CHUDEs.Select(cd => cd).Distinct().ToList();

            var distinctPublishers = db.NHAXUATBANs.Select(nxb => nxb).Distinct().ToList();

            var viewModel = new HomeViewModel
            {
                Books = books,
                DistinctSubjects = distinctSubjects,
                DistinctPublishers = distinctPublishers
            };

            return View(viewModel);
        }

        public ActionResult Publishers(int id)
        {
            // Retrieve the subject based on the provided id
            var subject = db.NHAXUATBANs.Find(id);
            if (subject == null)
            {
                return HttpNotFound(); // Return a 404 error if the subject is not found
            }

            // Retrieve all books with the same subject id (MaCD)
            var books = db.SACHes.Where(b => b.MaNXB == id).ToList();


            var distinctSubjects = db.CHUDEs.Select(cd => cd).Distinct().ToList();

            var distinctPublishers = db.NHAXUATBANs.Select(nxb => nxb).Distinct().ToList();

            var viewModel = new HomeViewModel
            {
                Books = books,
                DistinctSubjects = distinctSubjects,
                DistinctPublishers = distinctPublishers
            };

            return View(viewModel);
        }
    }
}