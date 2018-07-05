using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.xtra;
using iTextSharp.text.pdf;

namespace wflow
{
  public   class ImageEvent : IPdfPCellEvent
    {
        protected iTextSharp.text.Image img;
        public ImageEvent(iTextSharp.text.Image img)
        {
            this.img = img;
        }
        //protected public void setImg(iTextSharp.text.Image img)
        //{
        //    this.img = img;
        //}
        void IPdfPCellEvent.CellLayout(PdfPCell cell, iTextSharp.text.Rectangle position, PdfContentByte[] canvases)
        {
            img.ScaleToFit(position.Width, position.Height);

            //img.SetAbsolutePosition(position.Left + (position.Width - img.Width) / 2,
            //        position.Bottom + (position.Height - img.ScaledHeight / 2));
            img.SetAbsolutePosition(position.Left  , position.Bottom );
            //img.SetAbsolutePosition(0,0);
            PdfContentByte canvas = canvases[PdfPTable.BACKGROUNDCANVAS];
            try
            {
                canvas.AddImage(img);
            }
            catch (DocumentException ex)
            {
                // do nothing
            }
        }
    }
}
public  class PositionEvent : IPdfPCellEvent
{
    protected Phrase content;
    protected string pos;

    public PositionEvent(Phrase content, string pos)
    {
        this.content = content;
        this.pos = pos;
    }


    void IPdfPCellEvent.CellLayout(PdfPCell cell, iTextSharp.text.Rectangle position, PdfContentByte[] canvases)
    {
        PdfContentByte canvas = canvases[PdfPTable.TEXTCANVAS];
        float x = 0;
        float y = 0;
        int alignment = 0;
        switch (pos)
        {
            case "TOP_LEFT":
                x = position.GetLeft(3);
                y = position.GetTop(content.Leading);
                alignment = Element.ALIGN_LEFT;
                break;
            case "TOP_RIGHT":
                x = position.GetRight(3);
                y = position.GetTop(content.Leading);
                alignment = Element.ALIGN_RIGHT;
                break;
            case "BOTTOM_LEFT":
                x = position.GetLeft(3);
                y = position.GetBottom(3);
                alignment = Element.ALIGN_LEFT;
                break;
            case "BOTTOM_RIGHT":
                x = position.GetRight(3);
                y = position.GetBottom(3);
                alignment = Element.ALIGN_RIGHT;
                break;
            case "CENTER_TOP":
                x = position.GetRight(3) + position.GetLeft(3) / 2;
                y = position.GetTop(3);
                alignment = Element.ALIGN_RIGHT;
                break;
            case "CENTER_BOTTOM":
                x = position.GetRight(3) + position.GetLeft(3) / 2;
                y = position.GetBottom(3);
                alignment = Element.ALIGN_RIGHT;
                break;
            case "CENTER_MIDDLE":
                x = position.GetRight(3) + position.GetLeft(3) / 2;
                y = x;
                alignment = Element.ALIGN_RIGHT;
                break;
        }
        ColumnText.ShowTextAligned(canvas, alignment, content, x, y, 0);
    }

    //void IPdfPCellEvent.CellLayout(PdfPCell cell, iTextSharp.text.Rectangle position, PdfContentByte[] canvases)
    //{
    //    PdfContentByte canvas = canvases[PdfPTable.TEXTCANVAS];
    //    float x = 0;
    //    float y = 0;
    //    int alignment = 0;
    //    switch (pos)
    //    {
    //        case "TOP_LEFT":
    //            x = position.GetLeft(3);
    //            y = position.GetTop(content.Leading);
    //            alignment = Element.ALIGN_LEFT;
    //            break;
    //        case "TOP_RIGHT":
    //            x = position.GetRight(3);
    //            y = position.GetTop(content.Leading);
    //            alignment = Element.ALIGN_RIGHT;
    //            break;
    //        case "BOTTOM_LEFT":
    //            x = position.GetLeft(3);
    //            y = position.GetBottom(3);
    //            alignment = Element.ALIGN_LEFT;
    //            break;
    //        case "BOTTOM_RIGHT":
    //            x = position.GetRight(3);
    //            y = position.GetBottom(3);
    //            alignment = Element.ALIGN_RIGHT;
    //            break;
    //        case "CENTER_TOP":
    //            x = position.GetRight(3) + position.GetLeft(3) / 2;
    //            y = position.GetTop(3);
    //            alignment = Element.ALIGN_RIGHT;
    //            break;
    //        case "CENTER_BOTTOM":
    //            x = position.GetRight(3) + position.GetLeft(3) / 2;
    //            y = position.GetBottom(3);
    //            alignment = Element.ALIGN_RIGHT;
    //            break;
    //        case "CENTER_MIDDLE":
    //            x = position.GetRight(3) + position.GetLeft(3) / 2;
    //            y = x;
    //            alignment = Element.ALIGN_RIGHT;
    //            break;
    //    }
    //    ColumnText.ShowTextAligned(canvas, alignment, content, x, y, 0);
    //}
}
