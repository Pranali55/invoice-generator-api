from reportlab.lib.pagesizes import A4
from reportlab.lib import colors
from reportlab.lib.units import mm
from reportlab.platypus import SimpleDocTemplate, Table, TableStyle, Paragraph, Spacer
from reportlab.lib.styles import getSampleStyleSheet, ParagraphStyle
from reportlab.lib.enums import TA_RIGHT, TA_CENTER
import io
from models.schemas import InvoiceRequest

def generate_invoice_pdf(invoice: InvoiceRequest) -> bytes:
    buffer = io.BytesIO()
    doc = SimpleDocTemplate(buffer, pagesize=A4, rightMargin=15*mm, leftMargin=15*mm, topMargin=15*mm, bottomMargin=15*mm)
    styles = getSampleStyleSheet()
    story = []

    # Header
    header_style = ParagraphStyle("header", fontSize=22, fontName="Helvetica-Bold", textColor=colors.HexColor("#2563EB"))
    story.append(Paragraph(invoice.business_name, header_style))
    if invoice.business_address:
        story.append(Paragraph(invoice.business_address, styles["Normal"]))
    story.append(Spacer(1, 8*mm))

    # Invoice title + number
    title_style = ParagraphStyle("title", fontSize=16, fontName="Helvetica-Bold")
    right_style = ParagraphStyle("right", fontSize=11, alignment=TA_RIGHT)
    info_table = Table([
        [Paragraph("INVOICE", title_style), Paragraph(f"#{invoice.invoice_number}", right_style)],
        [Paragraph(f"Issue Date: {invoice.issue_date}", styles["Normal"]), Paragraph(f"Due Date: {invoice.due_date}", right_style)],
    ], colWidths=[90*mm, 90*mm])
    story.append(info_table)
    story.append(Spacer(1, 8*mm))

    # Bill To
    story.append(Paragraph("<b>Bill To:</b>", styles["Normal"]))
    story.append(Paragraph(invoice.client_name, styles["Normal"]))
    story.append(Paragraph(invoice.client_email, styles["Normal"]))
    story.append(Spacer(1, 8*mm))

    # Items table
    item_data = [["Description", "Qty", "Unit Price", "Total"]]
    for item in invoice.items:
        item_data.append([
            item.description,
            str(item.quantity),
            f"${item.unit_price:,.2f}",
            f"${item.line_total:,.2f}"
        ])

    item_table = Table(item_data, colWidths=[90*mm, 20*mm, 35*mm, 35*mm])
    item_table.setStyle(TableStyle([
        ("BACKGROUND", (0, 0), (-1, 0), colors.HexColor("#2563EB")),
        ("TEXTCOLOR", (0, 0), (-1, 0), colors.white),
        ("FONTNAME", (0, 0), (-1, 0), "Helvetica-Bold"),
        ("FONTSIZE", (0, 0), (-1, 0), 10),
        ("ALIGN", (1, 0), (-1, -1), "RIGHT"),
        ("ROWBACKGROUNDS", (0, 1), (-1, -1), [colors.white, colors.HexColor("#F1F5F9")]),
        ("GRID", (0, 0), (-1, -1), 0.5, colors.HexColor("#CBD5E1")),
        ("PADDING", (0, 0), (-1, -1), 6),
    ]))
    story.append(item_table)
    story.append(Spacer(1, 6*mm))

    # Totals
    totals = [
        ["Subtotal", f"${invoice.subtotal:,.2f}"],
        ["Tax", f"${invoice.tax:,.2f}"],
        ["Discount", f"-${invoice.discount:,.2f}"],
        ["TOTAL", f"${invoice.total:,.2f}"],
    ]
    totals_table = Table(totals, colWidths=[130*mm, 50*mm])
    totals_table.setStyle(TableStyle([
        ("ALIGN", (0, 0), (-1, -1), "RIGHT"),
        ("FONTNAME", (0, 3), (-1, 3), "Helvetica-Bold"),
        ("FONTSIZE", (0, 3), (-1, 3), 12),
        ("BACKGROUND", (0, 3), (-1, 3), colors.HexColor("#EFF6FF")),
        ("LINEABOVE", (0, 3), (-1, 3), 1, colors.HexColor("#2563EB")),
        ("PADDING", (0, 0), (-1, -1), 4),
    ]))
    story.append(totals_table)

    # Notes
    if invoice.notes:
        story.append(Spacer(1, 8*mm))
        story.append(Paragraph("<b>Notes:</b>", styles["Normal"]))
        story.append(Paragraph(invoice.notes, styles["Normal"]))

    doc.build(story)
    return buffer.getvalue()
