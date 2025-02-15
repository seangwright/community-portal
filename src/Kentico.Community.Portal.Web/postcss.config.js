import postcssPresentEnv from "postcss-preset-env";
import purgecss from "@fullhuman/postcss-purgecss";

/** @type { import('postcss-load-config').ConfigFn} */
const config = ({ env }) => ({
  plugins: [
    // https://purgecss.com/configuration.html
    env === "production" &&
      purgecss({
        content: ["./**/*.cshtml"],

        safelist: {
          standard: [
            // .note comes from markdown :::note syntax
            "note",
            // Syntax highlighting for inline code blocks
            "code",
            // Responsive embeds for youtube videos
            "embed-container",
            // Used in blog posts and Q&A
            "blockquote",
            "figure",
          ],
          greedy: [/form-/],
        },
      }),
    // https://www.npmjs.com/package/postcss-preset-env
    env === "production" && postcssPresentEnv(),
  ],
});

export default config;
