using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AozoraSharp.Logging;

namespace AozoraSharp.Core;

public abstract class LazyList<T>
{
    /// <summary>
    /// 取得済みアイテム
    /// </summary>
    private readonly List<T> acquiredItems = [];

    private readonly ILogger logger = LogManager.Instance.GetLogger(nameof(LazyList<T>));

    /// <summary>
    /// 追加のアイテムを取得し，アイテムがあれば<see cref="list"/>に追加する
    /// </summary>
    /// <returns>アイテムが追加されればtrue，追加されなければfalse</returns>
    private async Task<bool> AppendAdditionalItemsAsync(CancellationToken cancellationToken = default)
    {
        var additionalItems = await GetMoreAsync(cancellationToken);
        if (additionalItems == default || additionalItems.Count <= 0)
        {
            logger.Debug($"No more items (total: {acquiredItems.Count})");
            return false;
        }
        logger.Debug("getting more");
        acquiredItems.AddRange(additionalItems);
        return true;
    }
    protected abstract Task<IReadOnlyList<T>> GetMoreAsync(CancellationToken cancellationToken = default);

    public LazyEnumerator GetAsyncEnumerator() => new LazyEnumerator(this);

    public sealed class LazyEnumerator
    {
        private readonly LazyList<T> list;

        internal LazyEnumerator(LazyList<T> lazyList)
        {
            list = lazyList;
        }

        public T Current { get; private set; } = default;

        private int lastIndex = 0;

        public async ValueTask<bool> MoveNextAsync()
        {
            // 取得済みアイテムがもうなくて，追加取得もできなかった場合
            if (lastIndex >= list.acquiredItems.Count && !await list.AppendAdditionalItemsAsync())
            {
                Current = default;
                return false;
            }
            Current = list.acquiredItems[lastIndex++];
            return true;
        }
    }
}
