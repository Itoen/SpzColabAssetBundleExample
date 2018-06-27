# SpzColabAssetBundleExample
![assetbundlespzcolab](https://user-images.githubusercontent.com/17733911/41985634-d8bc0574-7a6e-11e8-858a-03de6881109f.gif)  
  
https://supporterzcolab.com/event/443/  
用のサンプルプロジェクトです。  
  
## 利用方法
AssetBundleManagerSingletonのIsInitializedがtrueになったら利用が可能です。  
  
- LoadFromCacheOrDownloadAssetBundleでアセットバンドルのダウンロード  
- LoadAssetAsyncでアセットの非同期読み込み  
が出来ます。
  
## 説明
- AssetBundleInfoディレクトリ下　　・・・ アセットバンドルに関する情報 & 情報を管理するクラス  
- AssetLoaderディレクトリ下　　　　・・・ アセット読み込みクラス  
- Downloaderディレクトリ下　　　　・・・ アセットバンドルのダウンロードクラス  
- OnMemoryディレクトリ下　　　　・・・ メモリ上のアセットバンドル管理クラス  
- Editorディレクトリ下　　　　　　　・・・ アセットバンドルビルドクラス
