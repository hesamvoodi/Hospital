using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Hospital.Models;

namespace Hospital.TagHelpers
{
    [HtmlTargetElement("div", Attributes = "page-model")]
    public class PageLinkTagHelper : TagHelper
    {
        private IUrlHelperFactory _urlHelperFactory;

        public PageLinkTagHelper(IUrlHelperFactory urlHelperFactory)
        {
            this._urlHelperFactory = urlHelperFactory;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }


        public PagingInfo PageModel { get; set; }
        public string PageAction { get; set; }
        public string PageClass { get; set; }
        public string PageClassNormal { get; set; }
        public string PageClassSelected { get; set; }
        public string PageClassFirstLastPage { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = this._urlHelperFactory.GetUrlHelper(ViewContext);
            //<div><div/>
            TagBuilder result = new TagBuilder("div");

            TagBuilder firstTag = new TagBuilder("a"); 
            string firstUrl = PageModel.UrlParam.Replace(":", "1");
            firstTag.Attributes["href"] = firstUrl;
            firstTag.AddCssClass(PageClass);
            firstTag.AddCssClass(PageClassFirstLastPage);
            firstTag.InnerHtml.Append("صفحه نخست");

            //<div><a></a></div>
            result.InnerHtml.AppendHtml(firstTag);

            for (int i = (PageModel.CurrentPage > 3 ? PageModel.CurrentPage - 2 : 1) ;
                i <= (PageModel.CurrentPage + 3 > PageModel.TotalPage ? PageModel.TotalPage : PageModel.CurrentPage + 3);
                i++)
            {
                //<a><a/>
                TagBuilder tag = new TagBuilder("a");
                string url = PageModel.UrlParam.Replace(":", i.ToString());
                tag.Attributes["href"] = url;
                tag.AddCssClass(PageClass);
                tag.AddCssClass(i == PageModel.CurrentPage ? PageClassSelected : PageClassNormal);
                tag.InnerHtml.Append(i.ToString());

                //<div><a></a></div>
                result.InnerHtml.AppendHtml(tag); 
            }
            TagBuilder lastTag = new TagBuilder("a");
            string lastUrl = PageModel.UrlParam.Replace(":", PageModel.TotalPage.ToString());
            lastTag.Attributes["href"] = lastUrl;
            lastTag.AddCssClass(PageClass);
            lastTag.AddCssClass(PageClassFirstLastPage);
            lastTag.InnerHtml.Append("صفحه آخر"); 

            //<div><a></a></div>
            result.InnerHtml.AppendHtml(lastTag);


            output.Content.AppendHtml(result.InnerHtml);
        }
    }
}
