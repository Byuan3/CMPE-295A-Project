from fastapi import FastAPI

app = FastAPI()


@app.get("/debug/hello_world", tags=["debug"])
async def hello_world(name: str = "the World") -> dict:
    return {"Message": f"Hello, {name}"}
