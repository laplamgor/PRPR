using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace PRPR.Common.Controls
{
    public partial class JustifiedWrapPanel
    {
        bool loading = false;


		public async Task CheckNeedMoreItemAsync()
        {
            if (ParentScrollViewer != null && ParentScrollViewer.VerticalOffset > ParentScrollViewer.ScrollableHeight - 1.5 * ParentScrollViewer.ViewportHeight &&
                !loading && ItemsSource is ISupportIncrementalLoading && ((ISupportIncrementalLoading)ItemsSource).HasMoreItems)
            {
                await LoadMoreItemsAsync();
            }
        }

        async Task LoadMoreItemsAsync()
        {
            // Usually load as many as current viewport has
			// So it just prepares for about one half active window
            await LoadMoreItemsAsync(Math.Max((uint)Containers.Count / 2, 10));
		}

        async Task LoadMoreItemsAsync(uint count)
        {
            var itemsSourceAtStart = ItemsSource;
            if (ItemsSource is ISupportIncrementalLoading items)
            {
				// Lock the loading so it wont be call repeatedly while waiting new items incoming
                if (!loading)
                {
                    loading = true;

					// Load as many as possible to meet the requested number
                    uint loaded = 0;
					while (items.HasMoreItems && loaded < count && itemsSourceAtStart == ItemsSource)
					{
                        try
                        {
                            loaded += (await items.LoadMoreItemsAsync(count)).Count;

                            // When new items are add, must recheck the realization
                            if (UpdateActiveRange(ParentScrollViewer.VerticalOffset, ParentScrollViewer.ViewportHeight, this.DesiredSize.Width - this.Margin.Left - this.Margin.Right, true))
                            {
                                RevirtualizeAll();
                            }
                            Debug.WriteLine($"LoadMoreItemsAsync");
                            //InvalidateMeasure();
                            //InvalidateArrange();
                        }
                        catch (Exception ex)
                        {
							// Stop it if there is any exception
                            break;
                        }
                    }

					
                    loading = false;
                }
            }
        }
    }
}
