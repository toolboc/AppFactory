using System;
using System.Collections.ObjectModel;
using System.Linq;
using Template.Config.Structure;

namespace Template.Config.Implementations
{
    public class MultiplePanePageImplementation : PageImplementation
    {
        public ObservableCollection<PaneImplementation> Items { get; private set; }


        public MultiplePanePageImplementation(IRepository repository, MultiplePanePageDefinition definition)
            : base(repository, definition)
        {

            Items = new ObservableCollection<PaneImplementation>(definition.Panes.Select<PaneDefinition, PaneImplementation>(p =>
                                                                                                                                {
                                                                                                                                    if (p is ListPaneDefinition)
                                                                                                                                    {
                                                                                                                                        return new ListPaneImplementation(repository, p as ListPaneDefinition);
                                                                                                                                    }
                                                                                                                                    if (p is LinkPaneDefinition)
                                                                                                                                    {
                                                                                                                                        return new LinkPaneImplementation(repository,
                                                                                                                                                                          p as LinkPaneDefinition);
                                                                                                                                    }
                                                                                                                                    throw new Exception("Unable to find implementation for pane definition [" + p.GetType().Name + "]");
                                                                                                                                }));
        }
    }
}