using System;
using FluentAssertions;
using NUnit.Framework;
using PodcastManager.FeedUpdater.Domain.Exceptions;
using PodcastManager.FeedUpdater.Domain.Interactors;
using PodcastManager.FeedUpdater.Domain.Models;
using PodcastManager.FeedUpdater.Doubles.HttpClientFactory;

namespace PodcastManager.FeedUpdater.Application.Services;

public class FeedConverterServiceTests
{
    private FeedConverterService service = null!;

    private void CreateService()
    {
        var newService = new FeedConverterService();
        service = newService;
    }

    [SetUp]
    public void SetUp() => CreateService();

    [Test]
    public void Constructor_InheritsFromFeedConverterInteractor() =>
        service.Should().BeAssignableTo<IFeedConverterInteractor>();

    [Test]
    public void Execute_IfStringIsEmptyReturnsEmptyFeed()
    {
        var result = service.Execute(string.Empty);
        result.Should().BeEquivalentTo(Feed.Empty());
    }

    [Test]
    public void Execute_GetInformationIfTheFeedHasNoEpisodes()
    {
        var feed = service.Execute(HttpClientFactoryStub.Data["withoutEpisodes"]);
        feed.Should().BeEquivalentTo(new Feed(
            "SWAOG Amateur Astronomy Network",
            "http://www.swaog.com",
            "The South West Astronomy Observers Group (SWAOG) is a group of Amateur Astronomers who host a weekly astronomy program via a Ham Radio Network",
            "en-us",
            new Image("http://astronomy.thebrownhouse.org/audio/swaog_itunes_logo_v3.jpg"),
            "Amateur Astronomy show hosted by amateur astronomers and radio operators",
            "The South West Astronomy Observers Group (SWAOG) is a group of Amateur Astronomers who host a weekly astronomy program via a Ham Radio Network",
            new Owner("Gary Brown (KC4VNU)", "junk@thebrownhouse.org")
        )
        {
            Categories = new[] { "Science & Medicine", "Sports & Recreation" },
        });
    }

    [Test]
    public void Execute_GetEpisodeInformationFromTheFeed()
    {
        var feed = service.Execute(HttpClientFactoryStub.Data["withEpisodes"]);
        feed.Should().BeEquivalentTo(new Feed(
            "Killer Innovations with Phil McKinney - A Show About Ideas Creativity And Innovation",
            "https://killerinnovations.com")
        {
            Items = new []
            {
                new Item (
                    "Peter Lierni of Solutioneering on Innovating Frameworks for Critical Thinking",
                    new DateTime(2022, 3, 8, 13, 0, 22),
                    "https://killerinnovations.com/?p=8153",
                    new Enclosure("http://traffic.libsyn.com/philmckinney/Peter_Lierni_of_Solutioneering_on_Innovating_Frameworks_for_Critical_Thinking.mp3", 70962894, "audio/mpeg"),
                    "https://killerinnovations.com/peter-lierni-of-solutioneering-on-innovating-frameworks-for-critical-thinking/",
                    "Peter Lierni of Solutioneering joins us on the second episode of Season 18. The Solution Engineering Tool (SET) is a framework that takes an engineering-based approach to win competitive deals. The Solution Engineering Tool As a Navy officer, Peter lived on a ship, which he described as a system of systems. After leaving the Navy, […]",
                    "Peter Lierni of Solutioneering joins us on the second episode of Season 18. The Solution Engineering Tool (SET) is a framework that takes an engineering-based approach to win competitive deals. The Solution Engineering Tool As a Navy officer,",
                    "Peter Lierni of Solutioneering joins us on the second episode of Season 18. The <a href=\"https://www.solutionengineeringtool.company/\">Solution Engineering Tool </a>(SET) is a framework that takes an engineering-based approach to win competitive deals.<br />\n<br />\nThe Solution Engineering Tool<br />\nAs a Navy officer, Peter lived on a ship, which he described as a system of systems. After leaving the Navy, he started consulting in the Pentagon, assessing complex weapons systems. Peter connected building a complex system and doing business development and capture and proposal planning. Peter began using different mental models he developed from his experiences to win deals. He was eventually encouraged to develop this into a <a href=\"https://killerinnovations.com/4-required-elements-of-an-innovation-framework-2/\">framework</a>, and the Solution Engineering Tool (SET) was born. SET is a tool of tools, holding sixty-one tools that companies of all sizes can use and for deals of all sizes and timeframes.<br />\nThe Importance of Critical Thinking<br />\nInnovators often fail to think enough about their innovation\'s value critically. This critical thinking failure is where SET comes into play. SET starts with an issue and key factor analysis. The goal is to <a href=\"https://killerinnovations.com/brainstorming-problem-statements-as-a-team/\">understand the problem</a>, why it\'s a problem, and identify the critical problems and why they are problems. After that, it\'s a matter of showing the value your innovation offers.<br />\nThe Engineering Behind the Framework<br />\nWhen SET is used, it stores a data lake of competitive intelligence. SET provides a digital and visual blueprint that federates all the thinking behind how you got to a win. Many companies can\'t repeat successes because they didn\'t document the process behind them. SET collects strategies behind wins to be built upon and repeated in the future. SET\'s root cause analysis helps identify areas of success and areas that need improvement. In the case of a loss, the compiled data can see where things went wrong. Peter says no matter what <a href=\"https://killerinnovations.com/9-types-of-innovation-tools/\">innovation tool</a> you use, it\'s not the tool that makes you successful. It is how you use it. SET is meant to facilitate collaborative analysis to help you make decisions on strategies you apply, potential teams to partner with, who you should hire, and give reasons as to why your company should win a deal.<br />\nTo know more about Peter Lierni and the Solution Engineering Tool (SET) framework, listen to this week\'s show: <a href=\"http://traffic.libsyn.com/philmckinney/Peter_Lierni_of_Solutioneering_on_Innovating_Frameworks_for_Critical_Thinking.mp3\">Peter Lierni of Solutioneering on Innovating Frameworks for Critical Thinking</a>.<br />\n<br />\nInnovation doesn\'t happen by accident. It takes planning, action, and the right tools to achieve game-changing innovations.<br />\nNeed help? Check out the aids/help over at<a href=\"http://innovation.tools/\"> innovation.tools</a>.<br />",
                    "Phil McKinney",
                    new TimeSpan(0, 49, 06), new Image("https://secureservercdn.net/104.238.68.130/afd.9e6.myftpupload.com/wp-content/uploads/powerpress/KI-logo-oneline-podcast-cover-wide-Square.jpg")),
                new Item (
                    "Ask Me Anything Q&A",
                    new DateTime(2022, 3, 1, 13, 0, 04),
                    "https://killerinnovations.com/?p=8136",
                    new Enclosure("http://traffic.libsyn.com/philmckinney/Season_18_Launch__Ask_Me_Anything_QA.mp3", 82130004, "audio/mpeg"),
                    "https://killerinnovations.com/ask-me-anything-qa/",
                    "Welcome to season 18 of the Killer Innovations podcast! We are thrilled to kick off the 18th year of Killer Innovations. This episode is dedicated to answering your questions about the podcast. Ask Me Anything: Killer Innovations Q&A What was the original impetus for the podcast? My mentor Bob Davis, who I attribute much of my […]",
                    "Welcome to season 18 of the Killer Innovations podcast! We are thrilled to kick off the 18th year of Killer Innovations. This episode is dedicated to answering your questions about the podcast. Ask Me Anything: Killer Innovations Q&A What was the origi...",
                    "Welcome to season 18 of the Killer Innovations <a href=\"https://philmckinney.libsyn.com\">podcast</a>! We are thrilled to kick off the 18th year of Killer Innovations. This episode is dedicated to answering your questions about the podcast.<br />\n<br />\nAsk Me Anything: Killer Innovations Q&A<br />\nWhat was the original impetus for the podcast?<br />\nMy mentor Bob Davis, who I attribute much of my success to, inspired me to pay it forward<br />\nWhat was your inspiration for the format and structure of the podcast?<br />\nI used to listen to a motivational speaker, Earl Nightingale\'s Insight Audio Magazine cassette tapes.<br />\nHow did you produce the first podcast?<br />\nI recorded it in a Marriott bathroom, using a $5 microphone attached to my laptop.<br />\nHow did the podcast get traction?<br />\nAdam Curry, an MTV VJ who had a podcast called Daily Source Code, promoted my show.<br />\nWhat would be some surprising &#8220;podcast history&#8221; that others would find interesting?<br />\nOdeo was a podcast directory that promoted my podcast on their show. Not too long after, the Apple podcast came out, killing Odeo. Odeo pivoted and eventually became Twitter.<br />\nWhat was the original audience for the podcast? – The original audience consisted of mostly tech people.<br />\nHas the target audience for the podcast changed over time?<br />\nYes, most early listeners were podcasters. Now people from all sorts of backgrounds are listening.<br />\nWhat makes your show different from other podcasts?<br />\nLongevity (averaged 40-45 episodes a year for seventeen years) and staying consistent with the content.<br />\nHow have you produced the show over such a long period?<br />\nThis show is my creative outlet.<br />\nWhat are some of your favorite episodes?<br />\n<a href=\"https://killerinnovations.com/geoffrey-moore-what-it-takes-to-be-an-innovation-leader/\">Geoffrey Moore</a> or <a href=\"https://killerinnovations.com/sept-27th-with-peter-guber/\">Peter Guber</a>.<br />\nWhat has been the biggest challenge with <a href=\"https://killerinnovations.com/how-i-turned-my-podcast-into-a-book-and-radio-show/\">the podcast</a>?<br />\nDealing with personal issues that impact getting a show out and maintaining consistency<br />\nWhat is your advice to today\'s podcasters?<br />\nDon\'t chase the numbers. Focus on your content, be consistent, and collaborate with other podcasters who you like.<br />\nHow can someone who listens to the podcast benefit from it?<br />\nThe podcast is encouraging and holds timeless content with a long/deep archive of episodes &#8211; take advantage of it.<br />\nWhat would you say is your most significant achievement with the podcast?<br />\nInspiring others to podcast, inspiring creatives to create, and inspiring innovators to invent<br />\nIs there something you experienced that was unexpected about your podcast?<br />\nFans were becoming friends – Woody, Seth (designer), and many others.<br />\nWhat was the most incredible experience with fans of the podcast?<br />\nThe listeners of the show threw me a party in London.<br />\nTo know more about the Killer Innovations Show, listen to this week\'s show: <a href=\"http://traffic.libsyn.com/philmckinney/Season_18_Launch__Ask_Me_Anything_QA.mp3\">Ask Me Anything Q&A</a>.<br />\n<br />\nInnovation doesn\'t happen by accident. It takes planning, action, and the right tools to achieve game-changing innovations.<br />\nNeed help? Check out the aids/help over at<a href=\"http://innovation.tools/\"> innovation.tools</a>.<br />",
                    "Phil McKinney",
                    new TimeSpan(0, 55, 54), new Image("https://secureservercdn.net/104.238.68.130/afd.9e6.myftpupload.com/wp-content/uploads/powerpress/KI-logo-oneline-podcast-cover-wide-Square.jpg")),
                new Item (
                    "Tim Bajarin of Creative Strategies on Work from Home (WFH), Apple and Silicon Valley",
                    new DateTime(2022, 2, 22, 13, 0, 11),
                    "https://killerinnovations.com/?p=8123",
                    new Enclosure("http://traffic.libsyn.com/philmckinney/Tim_Bajarin_of_Creative_Strategies_on_Work_from_Home_WFH_Apple_and_Silicon_Valley.mp3", 85145997, "audio/mpeg"),
                    "https://killerinnovations.com/tim-bajarin-of-creative-strategies-on-work-from-home-wfh-apple-and-silicon-valley/",
                    "COVID-19 greatly impacted technology efforts and continues to do so rapidly. Tim Bajarin joins us from Silicon Valley to discuss work from home shifts and the newest innovations from Apple. Tim Bajarin On WFH Shifts Over the last eighteen months, businesses have realized that bringing everybody back to the office isn’t realistic. People are comfortable […]",
                    "COVID-19 greatly impacted technology efforts and continues to do so rapidly. Tim Bajarin joins us from Silicon Valley to discuss work from home shifts and the newest innovations from Apple. Tim Bajarin On WFH Shifts Over the last eighteen months,",
                    "COVID-19 greatly impacted technology efforts and continues to do so rapidly. Tim Bajarin joins us from Silicon Valley to discuss work from home shifts and the newest innovations from Apple.<br />\n<br />\nTim Bajarin On WFH Shifts<br />\nOver the last eighteen months, businesses have realized that bringing everybody back to the office isn’t realistic. People are comfortable with the flexibility that working from home provides. Creative Strategies did several studies on this topic and discovered that people establish <a href=\"https://killerinnovations.com/bob-odonnell-of-technalysis-research-what-innovation-impacts-you/\">sophisticated work from home set-ups</a>. Because people want consistency with their office and work from home, IT directors face new challenges. Even when in-office, people are still connecting with clients and colleagues virtually.<br />\nBecause people now want studios in their homes for work, homeschooling kids, etc., the housing demand has skyrocketed. Architects are currently designing new custom homes that include studios. We are moving towards the hybrid environment being the norm. Large and small companies have been investing in sprucing up their offices for their employees to match their WFH settings.<br />\nShifts in Technology<br />\nCOVID has opened room for more <a href=\"https://killerinnovations.com/eric-yuan-and-the-future-of-zoom/\">innovative technologies</a>. People want better cameras, pushing laptop makers and other tech companies to make heavy improvements. Intel competitors have evolved, such as AMD and Qualcomm. Apple’s introduction of the M1 Chip has immensely shaken things up in the valley. The demand for Macs has increased immensely as well. <a href=\"https://killerinnovations.com/what-is-the-optimal-innovation-team-size/\">Apple</a> is innovating on so many fronts and has the patience to stay in it for the long run.<br />\nRegulatory Pressures<br />\nGovernment regulation has become more prevalent, specifically with the European Union, as it attempts to harness the growth of companies like Apple and Facebook. Tim says that Washington D.C does not understand technology, which has caused some issues. While they talk significantly about clamping down on things, the reality is that great economic challenges will arise if governing bodies step in.<br />\nAbout our Guest: Tim Bajarin<br />\nTim Bajarin is one of <a href=\"https://killerinnovations.com/tim-bajarin-future-innovation-platforms/\">the most recognized and sought-after global technology analysts</a>, futurists, and consultants. His fifty years in Silicon Valley have made him a voice that moves the market.<br />\n“With his writing and analysis being at the forefront of the digital revolution, Tim was one of the first to cover the personal computer industry and is considered one of the leading experts in the field of technology adoption life cycles. He is president of a technology-focused company, <a href=\"https://en.wikipedia.org/w/index.php?title=Creative_Strategies&action=edit&redlink=1\">Creative Strategies</a>, and a regular podcaster on <a href=\"https://techpinions.com/\">Tech.Pinions</a> (also broadcasted on The Innovators Network). He is a futurist and credited with predicting the desktop publishing revolution three years before it reached the market and multimedia.<br />\nBeen with Creative Strategies since 1981, Tim has served as a consultant to most of the leading hardware and software vendors in the industry (IBM, Apple, Microsoft, Nvidia, AMD, HP, Xerox, Compaq, Dell, AT&T, Microsoft, Polaroid, Lotus, Epson, Qualcomm, Toshiba, and numerous others).<br />\nTim Bajarin is on the technology advisory boards of IBM, Compaq, and Dell. (from <a href=\"https://en.wikipedia.org/wiki/Tim_Bajarin\">Wikipedia</a>)”<br />\nTo know more about Tim Bajarin, Work from Home (WFH), Apple, and Silicon Valley, listen to this week\'s show: <a href=\"http://traffic.libsyn.com/philmckinney/Tim_Bajarin_of_Creative_Strategies_on_Work_from_Home_WFH_Apple_and_Silicon...",
                    "Phil McKinney",
                    new TimeSpan(0, 59, 03), new Image("https://secureservercdn.net/104.238.68.130/afd.9e6.myftpupload.com/wp-content/uploads/powerpress/KI-logo-oneline-podcast-cover-wide-Square.jpg")),
                new Item (
                    "Bob O’Donnell of TECHnalysis Research — What Innovation Will Impact You?",
                    new DateTime(2022, 2, 15, 13, 0, 17),
                    "https://killerinnovations.com/?p=8109",
                    new Enclosure("http://traffic.libsyn.com/philmckinney/Bob_ODonnell_of_TECHnalysis_Research__What_Innovation_Will_Impact_You.mp3", 72814112, "audio/mpeg"),
                    "https://killerinnovations.com/bob-odonnell-of-technalysis-research-what-innovation-impacts-you/",
                    "It's no mystery that COVID-19 disrupted the technology industry. Bob O'Donnell joins us to discuss the innovation impacts of broadband and 5G. Innovation Impacts of Remote Work COVID-19 dramatically changed how we communicate, increasing the need for fast, consistent broadband. The broadband industry globally builds around two and a half years of capacity ahead of […]",
                    "It's no mystery that COVID-19 disrupted the technology industry. Bob O'Donnell joins us to discuss the innovation impacts of broadband and 5G. Innovation Impacts of Remote Work COVID-19 dramatically changed how we communicate,",
                    "It\'s no mystery that COVID-19 disrupted the technology industry. Bob O\'Donnell<a href=\"https://killerinnovations.com/on-the-cusp-of-innovation-bob-odonnell-surveys-ces-2019-s14-ep47/\"> joins us</a> to discuss the innovation impacts of broadband and 5G.<br />\n<br />\nInnovation Impacts of Remote Work<br />\nCOVID-19 dramatically changed how we communicate, increasing the need for fast, consistent broadband. The broadband industry globally builds around two and a half years of capacity ahead of time. High internet usage spikes from COVID resulted in <a href=\"https://killerinnovations.com/thoughts-on-innovation/\">broadband capacity</a> exhaustion 14 days after March 1, 2020. The spikes were due to people working from home and kids being home from school. Zoom, WebEx, Microsoft, etc., all came together to improve the problems.<br />\nA strong work-from-home environment is the byproduct of the shift from in-person to remote. To build out their mobile workstations, people invest in high-quality PCs, monitors, cameras, platforms, etc. Bob believes there will continue to be an &#8220;<a href=\"https://philmckinney.com/are-you-unconventional-enough-in-your-thinking/\">explosion of installs</a>&#8221; of different video calling platforms.<br />\nThe Accelerated Pace of Technology Innovation<br />\nCompetition in the tech world has seen accelerated growth since COVID-19 started. There are four or five video conferencing platforms most people are currently using.<a href=\"https://killerinnovations.com/eric-yuan-and-the-future-of-zoom/\"> Zoom</a>, for instance, released 320 new features last year. When it comes to microchips, things have never been more competitive. Intel and AMD are working with global governments on semiconductor and supply chain issues. Technological innovations are continuously rolling out.<br />\n5G Innovation Growth<br />\nWhen it comes to <a href=\"https://killerinnovations.com/innovation-in-5g-capabilities/\">5G innovation</a>, most of the world started out using mid-band frequencies. The U.S started with low and high-band frequencies. Mid-band turned out to be the fastest, leaving U.S 5G efforts off to a slow start. Like Y2K, everybody knew 5G was coming but did not plan for it properly. However, things are now starting to pick up.<br />\nThe world\'s leading proponents on 5G private networks, Vodafone Germany recently unveiled a private network at Porsche\'s main factory. Everything at the factory runs on a 5G network, which does real-time tuning on the car while it\'s on a test track. Bob is currently working on a study of private 5G and how companies are implementing it. Everybody is talking about private 5G and its potential impacts from chip manufacturers to software, devices, and networks.<br />\nAbout our Guest: Bob O\'Donnell<br />\nBob O\'Donnell is the President, Founder, and Chief Analyst of TECHnalysis Research. The firm\'s research and O\'Donnell\'s opinions are also regularly used by major media outlets, including Bloomberg TV, CNBC, CNN, Investor\'s Business Daily, the Wall Street Journal, Yahoo Finance, and more.<br />\nO\'Donnell writes regular columns for USAToday and Forbes and a weekly blog for Tech.pinions.com published on TechSpot, SeekingAlpha, and LinkedIn. Before founding TECHnalysis Research, Bob served as Program Vice President, Clients, and Displays for industry research firm IDC. He is a graduate of the University of Notre Dame.<br />\nTo know more about Bob O\'Donnell and innovation impacts, listen to this week\'s show: <a href=\"http://traffic.libsyn.com/philmckinney/Bob_ODonnell_of_TECHnalysis_Research__What_Innovation_Will_Impact_You.mp3\">Bob O\'Donnell of TECHnalysis Research — What Innovation Will Impact You?</a><br />",
                    "Phil McKinney",
                    new TimeSpan(0, 50, 29), new Image("https://secureservercdn.net/104.238.68.130/afd.9e6.myftpupload.com/wp-content/uploads/powerpress/KI-logo-oneline-podcast-cover-wide-Square.jpg"))
            }
        });
    }

    [Test]
    public void ConvertToDate_IfIsNullEmptyOrOnlySpacesThrowError()
    {
        Action actNull = () => FeedConverterService.ConvertToDate(null!);
        Action actEmpty = () => FeedConverterService.ConvertToDate(string.Empty);
        Action actWhiteSpace = () => FeedConverterService.ConvertToDate("  ");
        
        actNull.Should().Throw<ArgumentNullException>();
        actEmpty.Should().Throw<ArgumentNullException>();
        actWhiteSpace.Should().Throw<ArgumentNullException>();
    }

    [Test]
    [TestCase("Tue, 08 Mar 2022 13:00:22 +0000", "2022-03-08T13:00:22")]
    [TestCase("Thu, 23 Jul 2009 10:20:42 PDT", "2009-07-23T17:20:42")]
    [TestCase("Thu, 17 Nov 2016 14:42:39 PST", "2016-11-17T22:42:39")]
    [TestCase("Mon, 12 Jun 2016 10:00:00 -0800", "2016-06-12T18:00:00")]
    [TestCase("Tue, 11 Jul 2016 12:00:00 PST", "2016-07-11T20:00:00")]
    [TestCase("Mon, 15 Jan 2017 09:00:00 PST", "2017-01-15T17:00:00")]
    [TestCase("Mon, 23 May 2017 09:00:00 PST", "2017-05-23T17:00:00")]
    [TestCase("Tue, 9 Sep 2017 09:00:00 PST", "2017-09-09T17:00:00")]
    [TestCase("Mon, 11 Jan 2018 09:00:00 PST", "2018-01-11T17:00:00")]
    [TestCase("Mon, 28 Jan 2018 01:00:00 PST", "2018-01-28T09:00:00")]
    [TestCase("Tue, 28 Jul 2018 18:00:00 PST", "2018-07-29T02:00:00")]
    [TestCase("Mon, 23 Sep 2018 17:00:00 PST", "2018-09-24T01:00:00")]
    [TestCase("Mon, 23 Sep 2018 17:00:00 EDT", "2018-09-23T21:00:00")]
    [TestCase("Mon, 17 Feb 2019 00:00:00 PST", "2019-02-17T08:00:00")]
    [TestCase("Mon, 22 Oct 2019 18:00:00 PST", "2019-10-23T02:00:00")]
    [TestCase("Mon, 3 Jan 2020 01:00:00 PST", "2020-01-03T09:00:00")]
    [TestCase("Mon, 19 Jul 2020 01:00:00 PST", "2020-07-19T09:00:00")]
    [TestCase("Mon, 21 Jul 2020 01:00:00 PST", "2020-07-21T09:00:00")]
    [TestCase("Sun, 3 Mar 2021 17:30:00 PST", "2021-03-04T01:30:00")]
    [TestCase("Sat, 15 Aug 2021 15:00:00 PST", "2021-08-15T23:00:00")]
    [TestCase("Thu, 10 Nov 2017 10:00:00 GMT", "2017-11-10T10:00:00")]
    [TestCase("Thu, 17 Nov 2017 10:00:00 GMT", "2017-11-17T10:00:00")]
    [TestCase("Sun, 20 Jun 2021 04:01:43 CEST", "2021-06-20T02:01:43")]
    [TestCase("Sun 21 Mar 2021 01:57:57 CET", "2021-03-21T00:57:57")]
    public void ConvertToDate_PassingADateShouldReturnCorrectDateTime(string date, string expected)
    {
        var result = FeedConverterService.ConvertToDate(date);
        result.ToString("yyyy-MM-ddTHH:mm:ss").Should().Be(expected);
    }

    [Test]
    public void ConvertToDate_WithABadFormattedDateTimeThrowsError()
    {
        Action act = () => _ = FeedConverterService.ConvertToDate("AAA");
        act.Should().Throw<DateTimeBadFormattedException>();
    }

    [Test]
    [TestCase("123", 123)]
    [TestCase("81.4", 85354086)]
    [TestCase("", 0)]
    [TestCase(null, 0)]
    [TestCase("  ", 0)]
    [TestCase("41:20", 0)]
    public void ConvertToLength_PassingALengthShouldReturnCorrectDateTime(string text, int expected)
    {
        Action act = () => _ = FeedConverterService.ConvertToDate("AAA");
        act.Should().Throw<DateTimeBadFormattedException>();
    }

    [Test]
    public void ConvertToLength_WithABadFormattedDateTimeThrowsError()
    {
        Action act = () => _ = FeedConverterService.ConvertToDate("AAA");
        act.Should().Throw<DateTimeBadFormattedException>();
    }
}