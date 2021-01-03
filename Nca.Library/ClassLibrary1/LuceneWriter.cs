using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Lucene.Net.Util;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Nca.Lucene
{
    public class LuceneWriter
    {
        public LuceneWriter()
        {
            
        }
        private async Task CreateIndexWriterAsync(Func<IndexWriter,Task> funcAsync)
        {
            const LuceneVersion AppLuceneVersion = LuceneVersion.LUCENE_48;
            // Construct a machine-independent path for the index
            var basePath = Environment.GetFolderPath(
                Environment.SpecialFolder.CommonApplicationData);
            var indexPath = Path.Combine(basePath, "index");
            using var dir = FSDirectory.Open(indexPath);
            // Create an analyzer to process the text
            var analyzer = new StandardAnalyzer(AppLuceneVersion);
            // Create an index writer
            var indexConfig = new IndexWriterConfig(AppLuceneVersion, analyzer);
            using var writer = new IndexWriter(dir, indexConfig);
            await funcAsync(writer);
        }
        public async Task AddIndex()
        {
            var source = new
            {
                Name = "Kermit the Frog",
                FavoritePhrase = "The quick brown fox jumps over the lazy dog"
            };
            var doc = new Document
            {
                // StringField indexes but doesn't tokenize
                new StringField("name",
                    source.Name,
                    Field.Store.YES),
                new TextField("favoritePhrase",
                    source.FavoritePhrase,
                    Field.Store.YES)
            };

            await CreateIndexWriterAsync(async (writer)=>
            {
                writer.AddDocument(doc);
                writer.Flush(triggerMerge: false, applyAllDeletes: false);
            });
        }
    }
}
