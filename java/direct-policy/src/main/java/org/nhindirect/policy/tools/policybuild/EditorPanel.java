package org.nhindirect.policy.tools.policybuild;

import java.awt.BorderLayout;
import java.awt.Color;
import java.io.File;
import java.io.InputStream;
import java.util.concurrent.Executors;
import java.util.concurrent.ScheduledExecutorService;
import java.util.concurrent.TimeUnit;

import javax.swing.BorderFactory;
import javax.swing.JLabel;
import javax.swing.JPanel;
import javax.swing.JScrollPane;
import javax.swing.JSplitPane;
import javax.swing.JTextArea;
import javax.swing.border.BevelBorder;
import javax.swing.border.CompoundBorder;
import javax.swing.border.EmptyBorder;
import javax.swing.border.EtchedBorder;
import javax.swing.border.SoftBevelBorder;
import javax.swing.event.DocumentEvent;
import javax.swing.event.DocumentListener;

import org.apache.commons.io.FileUtils;
import org.apache.commons.io.IOUtils;
import org.nhindirect.policy.PolicyLexicon;
import org.nhindirect.policy.PolicyLexiconParser;
import org.nhindirect.policy.PolicyLexiconParserFactory;
import org.nhindirect.policy.PolicyParseException;
import org.nhindirect.policy.tools.policyvalidate.ValidatePanel;

public class EditorPanel extends JPanel 
{
	private static final long serialVersionUID = 414048933382044206L;

	protected JLabel fileNameLabel;
	
	protected JTextArea policyText;
	
	protected ScheduledExecutorService buildTaskSchedule;
	
	protected Boolean needsBuilding = false;
	
	protected File currentFile;
	
	protected ValidatePanel validatePanel;
	
	public EditorPanel()
	{
		super();
		
		initUI();
		
		addActions();
		
		buildTaskSchedule = Executors.newSingleThreadScheduledExecutor();
		
		buildTaskSchedule.scheduleAtFixedRate(new BuildTask(), 3, 1, TimeUnit.SECONDS);
	}
	
	protected void initUI()
	{
		setLayout(new BorderLayout());
		setBorder(new CompoundBorder( 
                new SoftBevelBorder(BevelBorder.LOWERED), new EmptyBorder(5,5,5,5)) ); 
		
		policyText = new JTextArea();
		policyText.setLineWrap(true);
		policyText.setWrapStyleWord(true);
		policyText.setEditable(true);
		
		final JScrollPane scrollPane = new JScrollPane(policyText); 
		scrollPane.setVerticalScrollBarPolicy(
                JScrollPane.VERTICAL_SCROLLBAR_AS_NEEDED);
		
		scrollPane.setBorder(BorderFactory.createEtchedBorder(EtchedBorder.LOWERED));
		
		fileNameLabel = new JLabel("*New File*");
		
		final JPanel reportPanel = new JPanel(new BorderLayout());
		reportPanel.add(fileNameLabel, BorderLayout.NORTH);
		reportPanel.add(scrollPane, BorderLayout.CENTER);
		
		//this.add(reportPanel, BorderLayout.CENTER);
		
		validatePanel = new ValidatePanel();
		validatePanel.setFeedMode(PolicyLexicon.SIMPLE_TEXT_V1, policyText.getDocument());
		
		final JSplitPane splitPane = new JSplitPane(JSplitPane.VERTICAL_SPLIT,
				reportPanel, validatePanel);
		
		splitPane.setDividerLocation(400);
		
		this.add(splitPane, BorderLayout.CENTER);
	}
	
	private void addActions()
	{
		policyText.getDocument().addDocumentListener(new DocumentListener() {
			public void insertUpdate(DocumentEvent e) 
			{
				setBuildingNeeded(true);
		    }
		    public void removeUpdate(DocumentEvent e) 
		    {
				setBuildingNeeded(true);
		    }
		    public void changedUpdate(DocumentEvent e) 
		    {
				setBuildingNeeded(true);
		    }
		});
	}
	
	public void loadFromFile(File file)
	{
		try
		{
			final String text = FileUtils.readFileToString(file);
			
			policyText.setText(text);
			currentFile = file;
			fileNameLabel.setText(file.getPath());
		}
		catch (Exception e)
		{
			e.printStackTrace();
		}
	}
	
	protected class BuildTask implements Runnable
	{
		protected PolicyLexiconParser parser;
		
		public BuildTask()
		{
			try
			{
				parser = PolicyLexiconParserFactory.getInstance(PolicyLexicon.SIMPLE_TEXT_V1);
			}
			catch (Exception e)
			{
				
			}
		}
		
		@Override
		public void run()
		{		
			if (isBuildingNeeded())
			{
				// build
				if (!policyText.getText().isEmpty())
				{
					final InputStream stream = IOUtils.toInputStream(policyText.getText());
					
					try
					{
						parser.parse(stream);
						policyText.setForeground(Color.BLACK);
					}
					catch (PolicyParseException e)
					{
						policyText.setForeground(Color.RED);
					}
					catch (Exception e)
					{
						e.printStackTrace();
					}
					
					IOUtils.closeQuietly(stream);
				}
				
				setBuildingNeeded(false);
			}
		}
	}
	
	protected boolean isBuildingNeeded()
	{
		synchronized(needsBuilding)
		{
			return needsBuilding;
		}
	}
	
	protected void setBuildingNeeded(boolean buildingNeeded)
	{
		synchronized(needsBuilding)
		{
			needsBuilding = buildingNeeded;;
		}
	}
}