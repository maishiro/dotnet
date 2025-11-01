# FastMCP 2.0 による MCPサーバー構築

このプロジェクトは、既存のWebAPI (dfSelfHostWebAPI) をFastMCP 2.0を使用してMCPサーバーに変換するサンプルです。

## 必要条件

- Python 3.8以上
- uv (Pythonパッケージマネージャ)
- Node.js (MCPインスペクター用)
- 既存のWebAPI (dfSelfHostWebAPI) が起動していること

## インストール手順

1. 仮想環境の作成:
```cmd
uv venv .venv
```

2. 仮想環境の有効化:
```cmd
.venv\Scripts\activate
```

3. 依存パッケージのインストール:
```cmd
uv pip install -r requirements.txt
```

4. FastMCPのバージョン確認:
```cmd
uv run fastmcp version
```

## 使用方法

### 1. WebAPIの準備
最初に、dfSelfHostWebAPIのWebAPIを起動しておく必要があります。

### 2. FastMCPの設定と起動
1. MCPインスペクターの起動 (別ターミナルで):
```cmd
npx @modelcontextprotocol/inspector
```

2. 接続情報

- Transport Type: `STDIO`
- Command: `D:/dotnet/dfSelfHostWebAPI/run-FastMCP2/.venv/Scripts/python.exe`
- Arguments: `D:/dotnet/dfSelfHostWebAPI/run-FastMCP2/main.py`

## テスト

テストには、MCPインスペクターを使用します。これにより、APIのエンドポイントやレスポンスを視覚的に確認できます。
