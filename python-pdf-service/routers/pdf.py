from fastapi import APIRouter
from fastapi.responses import Response
from models.schemas import InvoiceRequest
from services.pdf_generator import generate_invoice_pdf

router = APIRouter()

@router.post("/generate-pdf")
def generate_pdf(invoice: InvoiceRequest):
    pdf_bytes = generate_invoice_pdf(invoice)
    return Response(content=pdf_bytes, media_type="application/pdf")
