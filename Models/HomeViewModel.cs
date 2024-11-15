using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;

namespace WebApplicationTH.Models
{
    public class HomeViewModel
    {
        public IPagedList<SACH> Books { get; set; }
        public List<CHUDE> DistinctSubjects { get; set; }
        public List<NHAXUATBAN> DistinctPublishers { get; set; }
    }
}