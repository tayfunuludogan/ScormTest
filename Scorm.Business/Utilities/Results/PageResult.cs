using Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scorm.Business.Utilities.Results
{
	public class PageResult<T> :DataResult<T> where T : class,new()
	{
        public PageResult(T Data, int itemCount, int page = 0, int pageSize = 10,bool success = true,string message=""):base(Data,success,message)
		{
			var totalPage = (int)Math.Ceiling((decimal)itemCount / (decimal)pageSize);
			int currentPage = page == 0 ? 1 : page;

			var startPage = currentPage - 5;
			var endPage = currentPage + 4;
			if (startPage <= 0)
			{
				endPage -= (startPage - 1);
				startPage = 1;
			}
			if (endPage > totalPage)
			{
				endPage = totalPage;
				if (endPage > 10)
				{
					startPage = endPage - 9;
				}
			}

			TotalItems = itemCount;
			CurrentPage = currentPage;
			PageSize = pageSize;
			TotalPages = totalPage;
			StartPage = startPage;
			EndPage = endPage;
		}

        public int TotalItems { get; private set; }
		public int CurrentPage { get; private set; }
		public int PageSize { get; private set; }
		public int TotalPages { get; private set; }
		public int StartPage { get; private set; }
		public int EndPage { get; private set; }
	}
}
