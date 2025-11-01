import httpx
from fastmcp import FastMCP

api_base_url = "http://localhost:9000"
openapi_spec_url = "http://localhost:9000/swagger/docs/v1"

client = httpx.AsyncClient(base_url=api_base_url)
openapi_spec = httpx.get(openapi_spec_url).json()

mcp = FastMCP.from_openapi(
  openapi_spec=openapi_spec,
  client=client,
  name="HELLO API Server",
)

if __name__ == "__main__":
  mcp.run()
