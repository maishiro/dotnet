# dfSelfHostWebAPI

古いフレームワーク .NET Framework 4.6.2で実装されたセルフホスト型WebAPIのサンプルプロジェクトです。

## 開発環境要件

- Visual Studio 2022
- .NET Framework 4.6.2 SDK
  - VS2022をインストールする際に、個別のコンポーネントから「.NET Framework 4.6.2 開発ツール」を選択する必要がある
  - または、Visual Studio インストーラーから後から追加

## 特徴

- **セルフホスト**: IISを必要とせず、実行ファイルとして動作するWebAPI
- **.NET Framework 4.6.2**: 従来の.NET Frameworkを使用
- **SimpleInjector**: 依存性注入（DI）コンテナとして採用
- **Swagger**: APIドキュメント自動生成とテスト用UI
- **SimpleInjector.Integration.WebApi**: NuGetから追加

## 実行方法

1. プロジェクトをビルド
2. 実行ファイルを起動
3. `http://localhost:9000` でサーバーが起動
4. SwaggerUIは `http://localhost:9000/swagger` で確認可能
   - Swaggerを表示するには、ブラウザで次のアドレスを開きます: `http://localhost:9000/swagger/`
5. プログラムを実行するには、次のコマンドが必要です:
   ```
   netsh http add urlacl url=http://+:9000/ user=DOMAIN\ユーザー名
   ```

## エンドポイント

- `GET /api/hello/{name}`
  - 指定された名前に対して挨拶を返すサンプルAPI
  - Swagger UIで各APIの詳細な説明を確認できます

## 使用パッケージ

- SimpleInjector 5.5.0
- SimpleInjector.Integration.WebApi
- Swashbuckle.Core 5.6.0
- Microsoft.AspNet.WebApi.SelfHost 5.3.0

## SwaggerのXML生成設定

Swaggerでコメントを表示するために、以下の設定が必要：

1. ソリューションエクスプローラーでプロジェクトを右クリック
2. プロパティを選択
3. ビルドタブで「XMLドキュメントファイルを生成」にチェックを入れる

この設定により、ビルド時にXMLコメントファイルが生成され、Swaggerで表示されるようになります。