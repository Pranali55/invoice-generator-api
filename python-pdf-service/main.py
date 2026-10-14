from fastapi import FastAPI
from routers import pdf

app = FastAPI(title="Invoice PDF Service")
app.include_router(pdf.router)

@app.get("/health")
def health():
    return {"status": "ok"}
