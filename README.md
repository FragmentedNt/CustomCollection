# CustomCollection
自作したC#のコレクション関連

## CircularBuffer\<T>
* 最大サイズ(Capacity)を指定した循環バッファ
  * Capacityを指定して，とは言ったが実際は指定したCapacityよりも大きい2^nの値に変更される（12を指定したら2^4=16になる）
* Queue\<T>にサイズ制限を設けたようなイメージ
* \[int i]で要素にランダムアクセス可能
  * -Capacity <= i < Capacityの範囲で負のインデックスも許容する
  * 上記範囲外ではIndexOutOfRangeException
  * [0]が保持している中で最も古いデータ，[Count-1]が直近にEnqueueされたデータ
  * [-1]が直近にEnqueueされたデータ，[-Count]が最も古いデータとなる
* IEnumerable継承でforeach可能

## FixedConcurrentQueue\<T>
* 最大サイズ(Capacity)を指定したスレッドセーフなキュー
* 大部分をConcurrentQueueから継承
* Enqueue時に指定したCapacityを超える場合にTryDequeueしてからEnqueueするだけ
* 上と違ってランダムアクセスはないです
