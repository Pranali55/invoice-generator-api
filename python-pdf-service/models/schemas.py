from pydantic import BaseModel
from typing import List, Optional

class InvoiceItem(BaseModel):
    description: str
    quantity: int
    unit_price: float
    line_total: float

class InvoiceRequest(BaseModel):
    invoice_number: str
    business_name: str
    business_address: Optional[str] = None
    client_name: str
    client_email: str
    issue_date: str
    due_date: str
    items: List[InvoiceItem]
    subtotal: float
    tax: float
    discount: float
    total: float
    notes: Optional[str] = None
